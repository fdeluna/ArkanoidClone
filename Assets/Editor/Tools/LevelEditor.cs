using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditor
{
    // Level Bricks
    public GameObject[] LevelBricks;

    // Properties
    public LevelManager LevelInfo
    {
        set
        {
            if (value != null)
            {
                _levelInfo = value;
                _grid = new LevelGrid(_levelInfo);
            }
        }
    }

    // Prefab Paths
    private const string BRICKS_PATH = "Assets/Prefabs/Resources/Bricks";
    private const string BACKGROUND_MATERIALS_PATH = "Assets/Material/Background Materials";
    
    private LevelManager _levelInfo;
    
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

    // Grid Tool
    private LevelGrid _grid;

    // Bricks Prefab window
    private Vector2 _paleteWindowPosition;
    private int _selectedPrefabIndex = 0;
    private int _currentPrefabIndex = 0;
    private float _prefabPreviewWidth = 100f;
    private float _prefabPreviewHeight = 100f;
    private List<GameObject> _bricksPrefabs;
    private GameObject _selectedPrefab;


    public LevelEditor(LevelManager levelManager)
    {        
        LevelInfo = levelManager;
        _bricksPrefabs = EditorToolsUtils.GetPrefabsAtPath(BRICKS_PATH);
        _selectedPrefabIndex = EditorPrefs.GetInt("_selectedPrefabIndex", -1);
        LevelBricks = new GameObject[_levelInfo.LevelData.LevelWidth * _levelInfo.LevelData.LevelHeight];
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
        if (_levelInfo.LevelData != null)
        {
            LevelBricks = new GameObject[_levelInfo.LevelData.LevelWidth * _levelInfo.LevelData.LevelHeight];

            foreach (BrickPosition brickPosition in _levelInfo.LevelData.LevelBricks)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Resources/Bricks/" + brickPosition.PrefabName + ".prefab", typeof(GameObject)) as GameObject;
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.transform.position = brickPosition.Position;
                go.transform.parent = _levelInfo.Bricks;

                Vector2Int gridPosition = _grid.WorldPositionToGrid(brickPosition.Position);
                LevelBricks[gridPosition.x + gridPosition.y * _levelInfo.LevelData.LevelWidth] = go;
            }
        }
    }

    public void UpdateTool()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        _grid.DrawGrid();
        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(50f, 100f, 225f, 360f), DrawPrefabPreviewWindow, "Level Editor");

        Handles.EndGUI();
    }

    public void OnMouseMove(Vector3 mousePosition)
    {
        if (Selection.activeTransform == _levelInfo.transform)
        {
            Vector3 worldPosition = _grid.MousePositionToWorldPosition(mousePosition);
            if (EraseMode)
            {
                EditorToolsUtils.DrawRectangle(worldPosition, _levelInfo.LevelData.BrickWidth, _levelInfo.LevelData.BrickHeight, new Color32(255, 77, 77, 70), Color.black);
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
            Vector2 gridPosition = _grid.MousePositionToGridPosition(mousePosition);
            GameObject brickAtPosition = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            brickAtPosition.transform.parent = _levelInfo.Bricks;
            brickAtPosition.transform.position = _grid.MousePositionToWorldPosition(mousePosition);
            LevelBricks[(int)gridPosition.x + (int)gridPosition.y * _levelInfo.LevelData.LevelWidth] = brickAtPosition;
        }
    }

    private GameObject GetSceneBrick(Vector3 position)
    {
        GameObject brick = null;

        Vector2 gridPosition = _grid.MousePositionToGridPosition(position);

        if (LevelBricks[(int)gridPosition.x + (int)gridPosition.y * _levelInfo.LevelData.LevelWidth] != null)
        {
            brick = LevelBricks[(int)gridPosition.x + (int)gridPosition.y * _levelInfo.LevelData.LevelWidth];
        }
        return brick;
    }

    private void DrawPrefabPreviewWindow(int windowID)
    {

        EditorToolsUtils.DrawScrollViewWindow(windowID, ref _paleteWindowPosition, ref _selectedPrefabIndex, _bricksPrefabs, _prefabPreviewWidth, _prefabPreviewHeight);
        EditorPrefs.SetInt("_selectedPrefabIndex", _selectedPrefabIndex);
        GetSelectedPrefab(_selectedPrefabIndex);
    }

    private void GetSelectedPrefab(int index)
    {
        if (index != -1 && _currentPrefabIndex != index)
        {
            _currentPrefabIndex = index;
            GameObject.DestroyImmediate(_selectedPrefab);
            _selectedPrefab = PrefabUtility.InstantiatePrefab(_bricksPrefabs[index]) as GameObject;
            _selectedPrefab.transform.parent = _levelInfo.transform;
            _selectedPrefab.hideFlags = HideFlags.HideInHierarchy;
            _selectedPrefab.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    //// TODO MOVE TO BACKGROUND WHATEVER TOOL

    //// TODO MOVE TO BACKGROUND WHATEVER TOOL
    //private Vector3 _backgroundMaterialWindowPosition;
    //private int _selectedIndexMaterial = -1;

    //private void DrawBackGroundMaterialWindow(int windowID)
    //{
    //    _backgroundMaterialWindowPosition = GUILayout.BeginScrollView(_backgroundMaterialWindowPosition);
    //    _selectedIndexMaterial = GUILayout.SelectionGrid(
    //        _selectedIndexMaterial,
    //        GetGUIContentsFromBackgroundMaterials(),
    //        4,
    //        GetGUIStyle()
    //        );
    //    GUILayout.EndScrollView();
    //    //EditorPrefs.SetInt("_selectedPrefabIndex", _selectedPrefabIndex);
    //    //GetSelectedItem(_selectedPrefabIndex);
    //}

    //private GUIContent[] GetGUIContentsFromBackgroundMaterials()
    //{
    //    List<GUIContent> guiContents = new List<GUIContent>();
    //    List<Material> materials = EditorToolsUtils.GetBackGroundMaterialsAtPath(BACKGROUND_MATERIALS_PATH);
    //    if (materials.Count > 0)
    //    {
    //        foreach (Material prefab in materials)
    //        {
    //            GUIContent guiContent = new GUIContent
    //            {
    //                text = prefab.name,
    //                image = AssetPreview.GetAssetPreview(prefab)
    //            };
    //            guiContents.Add(guiContent);
    //        }
    //    }
    //    return guiContents.ToArray();
    //}
}