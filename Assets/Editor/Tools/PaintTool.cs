using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PaintTool
{
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

    private LevelInfo _levelManager;

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

    public PaintTool(LevelInfo levelManager)
    {
        _levelManager = levelManager;
        _bricksPrefabs = ToolsUtils.GetPrefabsAtPath(BRICKS_PATH);
        _selectedPrefabIndex = EditorPrefs.GetInt("_selectedPrefabIndex", -1);
    }

    public void Reset()
    {
        GameObject.DestroyImmediate(_selectedPrefab);
        EditorPrefs.DeleteKey("_selectedPrefabIndex");
        _selectedPrefabIndex = -1;
    }

    public void UpdateTool()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        DrawGrid();
        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(50f, 100f, 225f, 360f), DrawPrefabPreviewWindow, "Level Editor");
        Handles.EndGUI();
    }

    public void LoadEditor()
    {
        if (_levelManager.LevelData != null && _levelManager.Bricks.childCount != _levelManager.LevelData.LevelBricks.Length)
        {
            for (int x = 0; x < _levelManager.LevelData.LevelWidth; x++)
            {
                for (int y = 0; y < _levelManager.LevelData.LevelHeight; y++)
                {
                    if (_levelManager.LevelData.LevelBricks[x + y * _levelManager.LevelData.LevelWidth] != null)
                    {
                        GameObject brick = _levelManager.LevelData.LevelBricks[x + y * _levelManager.LevelData.LevelWidth];
                        CreateBrickAtPosition(brick.transform.position, brick);
                    }
                }
            }
        }
    }

    private void DrawGrid()
    {
        _offSetPosition = new Vector3(_levelManager.transform.position.x + _levelManager.LevelData.BrickWidth / 2, _levelManager.transform.position.y - _levelManager.LevelData.BrickHeight / 2);
        for (int x = 0; x < _levelManager.LevelData.LevelWidth; x++)
        {
            for (int y = 0; y < _levelManager.LevelData.LevelHeight; y++)
            {
                Vector3 pos = new Vector3(_offSetPosition.x + x * _levelManager.LevelData.BrickWidth, _offSetPosition.y - y * _levelManager.LevelData.BrickHeight);
                ToolsUtils.DrawRectangle(pos, _levelManager.LevelData.BrickWidth, _levelManager.LevelData.BrickHeight, Color.clear, Color.white);
            }
        }
    }

    public void OnMouseMove(Vector3 mousePosition)
    {
        if (Selection.activeTransform == _levelManager.transform)
        {
            Vector3 worldPosition = MousePositionToWorldPosition(mousePosition);
            if (EraseMode)
            {
                ToolsUtils.DrawRectangle(worldPosition, _levelManager.LevelData.BrickWidth, _levelManager.LevelData.BrickHeight, new Color32(255, 77, 77, 70), Color.black);
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
            brickAtPosition.transform.parent = _levelManager.Bricks;
            brickAtPosition.transform.position = MousePositionToWorldPosition(mousePosition);
            _levelManager.LevelData.LevelBricks[(int)gridPosition.x + (int)gridPosition.y * _levelManager.LevelData.LevelWidth] = brickAtPosition;
        }
    }

    private GameObject GetSceneBrick(Vector3 position)
    {
        GameObject brick = null;

        Vector2 gridPosition = MousePositionToGridPosition(position);

        if (_levelManager.LevelData.LevelBricks[(int)gridPosition.x + (int)gridPosition.y * _levelManager.LevelData.LevelWidth] != null)
        {
            brick = _levelManager.LevelData.LevelBricks[(int)gridPosition.x + (int)gridPosition.y * _levelManager.LevelData.LevelWidth];
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

    private Vector2 WorldPositionToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((worldPosition.x - _offSetPosition.x) / _levelManager.LevelData.BrickWidth, 0, _levelManager.LevelData.LevelWidth - 1));
        int y = Mathf.RoundToInt(Mathf.Clamp((-worldPosition.y + _offSetPosition.y) / _levelManager.LevelData.BrickHeight, 0, _levelManager.LevelData.LevelHeight - 1));

        return new Vector2(x, y);
    }

    private Vector3 GridToWorldPosition(Vector2 gridPosition)
    {
        Vector3 pos = new Vector3
        {
            x = _levelManager.transform.position.x + (gridPosition.x * _levelManager.LevelData.BrickWidth + _levelManager.LevelData.BrickWidth / 2.0f),
            y = _levelManager.transform.position.y - (gridPosition.y * _levelManager.LevelData.BrickHeight + _levelManager.LevelData.BrickHeight / 2.0f)
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
            _selectedPrefab.transform.parent = _levelManager.transform;
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