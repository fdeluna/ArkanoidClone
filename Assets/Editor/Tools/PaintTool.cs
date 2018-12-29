using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LevelData;

[Serializable]
public class PaintTool
{
    public LevelInfo LevelManager;

    private const string BRICKS_PATH = "Assets/Prefabs/Resources/Bricks";

    private bool EraseMode
    {
        get
        {

            _eraseMode = Event.current.control;

            if (_selectedPrefab != null)
            {
                _selectedPrefab.SetActive(!_eraseMode);
            }
            return _eraseMode;
        }
    }
    private bool _eraseMode = false;


    // Grid offset
    private Vector3 _offSetPosition;

    // Level editor window
    private Vector3 _paleteWindowPosition;
    private int _selectedPrefabIndex = -1;
    private int _currentPrefabIndex = -1;
    private float _prefabPreviewWidth = 100f;
    private float _prefabPreviewHeight = 100f;
    private List<GameObject> _bricksPrefabs;
    private GameObject _selectedPrefab;

    // TODO MOVE TO PAINT TOOL
    public GameObject[] LevelBricks;

    public PaintTool(LevelInfo levelManager)
    {
        LevelManager = levelManager;
        _bricksPrefabs = EditorToolsUtils.GetPrefabsAtPath(BRICKS_PATH);
        _selectedPrefabIndex = EditorPrefs.GetInt("_selectedPrefabIndex", -1);
        _offSetPosition = new Vector3(LevelManager.transform.position.x + LevelManager.LevelData.BrickWidth / 2, LevelManager.transform.position.y - LevelManager.LevelData.BrickHeight / 2);
        LevelBricks = new GameObject[LevelManager.LevelData.LevelWidth * LevelManager.LevelData.LevelHeight];
        LoadEditor();
    }

    public void Reset()
    {
        GameObject.DestroyImmediate(_selectedPrefab);
        EditorPrefs.DeleteKey("_selectedPrefabIndex");
        _selectedPrefabIndex = -1;
    }

    public void LoadEditor()
    {        
        if (LevelManager.LevelData != null)
        {
            Debug.Log("Level Loaded");
            LevelBricks = new GameObject[LevelManager.LevelData.LevelWidth * LevelManager.LevelData.LevelHeight];

            foreach (BrickPosition brickPosition in LevelManager.LevelData.LevelBricks)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Resources/Bricks/" + brickPosition.PrefabName + ".prefab", typeof(GameObject)) as GameObject;
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.transform.position = brickPosition.Position;
                go.transform.parent = LevelManager.Bricks;

                Vector2Int gridPosition = WorldPositionToGrid(brickPosition.Position);
                LevelBricks[gridPosition.x + gridPosition.y * LevelManager.LevelData.LevelWidth] = go;
            }
        }
    }

    public void UpdateTool()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        DrawGrid();
        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(50f, 100f, 225f, 360f), DrawPrefabPreviewWindow, "Level Editor");
        Handles.EndGUI();
    }

    private void DrawGrid()
    {        
        for (int x = 0; x < LevelManager.LevelData.LevelWidth; x++)
        {
            for (int y = 0; y < LevelManager.LevelData.LevelHeight; y++)
            {
                Vector3 pos = new Vector3(_offSetPosition.x + x * LevelManager.LevelData.BrickWidth, _offSetPosition.y - y * LevelManager.LevelData.BrickHeight);
                EditorToolsUtils.DrawRectangle(pos, LevelManager.LevelData.BrickWidth, LevelManager.LevelData.BrickHeight, Color.clear, Color.white);
            }
        }
    }

    public void OnMouseMove(Vector3 mousePosition)
    {
        if (Selection.activeTransform == LevelManager.transform)
        {
            Vector3 worldPosition = MousePositionToWorldPosition(mousePosition);
            if (EraseMode)
            {
                EditorToolsUtils.DrawRectangle(worldPosition, LevelManager.LevelData.BrickWidth, LevelManager.LevelData.BrickHeight, new Color32(255, 77, 77, 70), Color.black);
            }
            else if (_selectedPrefab != null)
            {
                _selectedPrefab.transform.position = worldPosition;
            }
        }
    }

    public void OnMouseDown(Vector3 mousePosition)
    {
        if (EraseMode)
        {
            DeleteBrickAtPosition(mousePosition);
        }
        else if (_selectedPrefab != null)
        {
            CreateBrickAtPosition(mousePosition, _bricksPrefabs[_selectedPrefabIndex]);
        }
    }

    private void DeleteBrickAtPosition(Vector3 mousePosition)
    {
        GameObject brickAtPosition = GetSceneBrick(mousePosition);

        if (brickAtPosition != null)
        {
            GameObject.DestroyImmediate(brickAtPosition);
        }
    }

    private void CreateBrickAtPosition(Vector3 mousePosition, GameObject prefab)
    {
        DeleteBrickAtPosition(mousePosition);
        if (prefab != null)
        {
            Vector2 gridPosition = MousePositionToGridPosition(mousePosition);
            GameObject brickAtPosition = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            brickAtPosition.transform.parent = LevelManager.Bricks;
            brickAtPosition.transform.position = MousePositionToWorldPosition(mousePosition);
            LevelBricks[(int)gridPosition.x + (int)gridPosition.y * LevelManager.LevelData.LevelWidth] = brickAtPosition;
        }
    }

    private GameObject GetSceneBrick(Vector3 position)
    {
        GameObject brick = null;

        Vector2 gridPosition = MousePositionToGridPosition(position);

        if (LevelBricks[(int)gridPosition.x + (int)gridPosition.y * LevelManager.LevelData.LevelWidth] != null)
        {
            brick = LevelBricks[(int)gridPosition.x + (int)gridPosition.y * LevelManager.LevelData.LevelWidth];
        }
        return brick;
    }

    private Vector3 MousePositionToWorldPosition(Vector3 mousePosition)
    {
        Camera camera = SceneView.currentDrawingSceneView.camera;
        mousePosition.y = camera.pixelHeight - mousePosition.y;
        Vector2 gridPosition = WorldPositionToGrid(camera.ScreenToWorldPoint(mousePosition));
        return GridToWorldPosition(gridPosition);
    }

    private Vector2 MousePositionToGridPosition(Vector3 mousePosition)
    {
        Camera camera = SceneView.currentDrawingSceneView.camera;
        mousePosition.y = camera.pixelHeight - mousePosition.y;
        return WorldPositionToGrid(camera.ScreenToWorldPoint(mousePosition));
    }

    private Vector2Int WorldPositionToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((worldPosition.x - _offSetPosition.x) / LevelManager.LevelData.BrickWidth, 0, LevelManager.LevelData.LevelWidth - 1));
        int y = Mathf.RoundToInt(Mathf.Clamp((-worldPosition.y + _offSetPosition.y) / LevelManager.LevelData.BrickHeight, 0, LevelManager.LevelData.LevelHeight - 1));

        return new Vector2Int(x, y);
    }

    private Vector3 GridToWorldPosition(Vector2 gridPosition)
    {
        Vector3 pos = new Vector3
        {
            x = LevelManager.transform.position.x + (gridPosition.x * LevelManager.LevelData.BrickWidth + LevelManager.LevelData.BrickWidth / 2.0f),
            y = LevelManager.transform.position.y - (gridPosition.y * LevelManager.LevelData.BrickHeight + LevelManager.LevelData.BrickHeight / 2.0f)
        };

        return pos;
    }

    private void DrawPrefabPreviewWindow(int windowID)
    {
        _paleteWindowPosition = GUILayout.BeginScrollView(_paleteWindowPosition);
        _selectedPrefabIndex = GUILayout.SelectionGrid(
            _selectedPrefabIndex,
            GetGUIContentsFromPrefabs(_bricksPrefabs),
            2,
            GetGUIStyle()
            );
        GUILayout.EndScrollView();
        EditorPrefs.SetInt("_selectedPrefabIndex", _selectedPrefabIndex);
        GetSelectedItem(_selectedPrefabIndex);
    }

    private void GetSelectedItem(int index)
    {
        if (index != -1 && _currentPrefabIndex != index)
        {
            _currentPrefabIndex = index;
            GameObject.DestroyImmediate(_selectedPrefab);
            _selectedPrefab = PrefabUtility.InstantiatePrefab(_bricksPrefabs[index]) as GameObject;
            _selectedPrefab.transform.parent = LevelManager.transform;
            _selectedPrefab.hideFlags = HideFlags.HideInHierarchy;
            _selectedPrefab.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    private GUIContent[] GetGUIContentsFromPrefabs(List<GameObject> prefabs)
    {
        List<GUIContent> guiContents = new List<GUIContent>();
        if (prefabs.Count > 0)
        {
            foreach (GameObject prefab in prefabs)
            {
                GUIContent guiContent = new GUIContent
                {
                    text = prefab.name,
                    image = AssetPreview.GetAssetPreview(prefab)
                };
                guiContents.Add(guiContent);
            }
        }
        return guiContents.ToArray();
    }

    private GUIStyle GetGUIStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button)
        {
            imagePosition = ImagePosition.ImageAbove,
            alignment = TextAnchor.UpperCenter,
            fixedWidth = _prefabPreviewWidth,
            fixedHeight = _prefabPreviewHeight
        };
        return guiStyle;
    }
}