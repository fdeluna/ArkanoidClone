using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorTool
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
                LoadEditor();
            }
        }
    }

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

    #region Bricks Prefab Windows
    // Bricks Prefab window
    private Vector2 _paleteWindowPosition;
    private int _selectedPrefabIndex = 0;
    private int _currentPrefabIndex = 0;
    private float _prefabPreviewWidth = 100f;
    private float _prefabPreviewHeight = 100f;
    private List<GameObject> _bricksPrefabs;
    private GameObject _selectedPrefab;
    #endregion

    #region Background Windows
    // Bricks Prefab window
    private Vector2 _backGroundWindowPosition;
    private int _selectedBackgroundMaterialIndex = 0;
    private int _currentBackgroundMaterialIndex = 0;
    private float _backgroundPreviewWidth = 100f;
    private float _backgroundPreviewHeight = 100f;
    private List<Sprite> _backGroundMaterials;
    private Material _selectedBackgrounMaterial;
    #endregion

    public LevelEditorTool(LevelManager levelManager)
    {
        LevelInfo = levelManager;

        _bricksPrefabs = Utils.GetPrefabsAtPath(Utils.BRICKS_PATH);
        _backGroundMaterials = Utils.GetBackGroundMaterialsAtPath(Utils.BACKGROUND_MATERIALS_PATH);
        _selectedPrefabIndex = EditorPrefs.GetInt("_selectedPrefabIndex", -1);
        _selectedBackgroundMaterialIndex = EditorPrefs.GetInt("_selectedBackgroundMaterialIndex", -1);

        
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
        Debug.Log("Load Level");
        if (_levelInfo.LevelData != null)
        {
            Debug.Log(_levelInfo.LevelData.name);
            LevelBricks = new GameObject[LevelData.LevelWidth * LevelData.LevelHeight];

            foreach (BrickPosition brickPosition in _levelInfo.LevelData.LevelBricks)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Resources/Bricks/" + brickPosition.PrefabName + ".prefab", typeof(GameObject)) as GameObject;
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.transform.position = brickPosition.Position;
                go.transform.parent = _levelInfo.Bricks;

                Vector2Int gridPosition = _grid.WorldPositionToGrid(brickPosition.Position);
                LevelBricks[gridPosition.x + gridPosition.y * LevelData.LevelWidth] = go;
            }
            ChangeBackground(_levelInfo.LevelData.BackgroundSprite);
        }
    }

    public void UpdateTool()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        _grid.DrawGrid();
        Handles.BeginGUI();

        GUILayout.Window(0, new Rect(15f, 20f, 135f, Camera.current.pixelRect.height - 20f), DrawPrefabPreviewWindow, "Level Editor");
        GUILayout.Window(1, new Rect(Camera.current.pixelRect.width - 150f, 20f, 135f, Camera.current.pixelRect.height - 20f), DrawBackGroundWindow, "Background Materials");

        Handles.EndGUI();
    }

    #region Level editor interactions

    public void OnMouseMove(Vector3 mousePosition)
    {
        if (Selection.activeTransform == _levelInfo.transform)
        {
            Vector3 worldPosition = _grid.MousePositionToWorldPosition(mousePosition);
            if (EraseMode)
            {
                EditorToolsUtils.DrawRectangle(worldPosition, LevelData.BrickWidth, LevelData.BrickHeight, new Color32(255, 77, 77, 70), Color.black);
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
            LevelBricks[(int)gridPosition.x + (int)gridPosition.y * LevelData.LevelWidth] = brickAtPosition;
        }
    }

    private GameObject GetSceneBrick(Vector3 position)
    {
        GameObject brick = null;

        Vector2 gridPosition = _grid.MousePositionToGridPosition(position);

        if (LevelBricks[(int)gridPosition.x + (int)gridPosition.y * LevelData.LevelWidth] != null)
        {
            brick = LevelBricks[(int)gridPosition.x + (int)gridPosition.y * LevelData.LevelWidth];
        }
        return brick;
    }
    #endregion


    #region Scene Editor Windows

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

    private void DrawBackGroundWindow(int windowID)
    {

        EditorToolsUtils.DrawScrollViewWindow(windowID, ref _backGroundWindowPosition, ref _selectedBackgroundMaterialIndex, _backGroundMaterials, _backgroundPreviewWidth, _backgroundPreviewHeight);
        EditorPrefs.SetInt("_selectedBackgroundMaterialIndex", _selectedBackgroundMaterialIndex);
        GetSelectedBackgroundMaterial(_selectedBackgroundMaterialIndex);
    }

    private void GetSelectedBackgroundMaterial(int index)
    {
        if (index != -1)
        {
            _currentBackgroundMaterialIndex = index;
            ChangeBackground(_backGroundMaterials[_currentBackgroundMaterialIndex]);
        }
    }


    private void ChangeBackground(Sprite backgroundSprite)
    {
        _levelInfo.LevelData.BackgroundSprite = backgroundSprite;
        _levelInfo.Background.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
    }

    #endregion
}