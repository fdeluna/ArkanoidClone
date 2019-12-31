using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Level;
using Manager;
using UnityEditor;
using UnityEngine;

public class LevelEditorTool
{
    // Level Bricks
    public GameObject[] LevelBricks;

    // Properties  
    private ArkanoidManager _arkanoidManager;

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

    public LevelEditorTool(ArkanoidManager arkanoidManager)
    {
        _arkanoidManager = arkanoidManager;

        _grid = new LevelGrid(_arkanoidManager);
        _bricksPrefabs = Utils.GetPrefabsAtPath(Utils.BricksPath);
        _backGroundMaterials = Utils.GetBackGroundMaterialsAtPath(Utils.BackgroundMaterialsPath);
        _selectedPrefabIndex = EditorPrefs.GetInt("_selectedPrefabIndex", -1);
        _selectedBackgroundMaterialIndex = EditorPrefs.GetInt("_selectedBackgroundMaterialIndex", -1);

        LoadEditor();
    }

    public void Reset()
    {
        GameObject.DestroyImmediate(_selectedPrefab);
        EditorPrefs.DeleteKey("_selectedPrefabIndex");
        _selectedPrefabIndex = 0;
    }

    public void LoadEditor()
    {
        Debug.Log("Load Level");
        if (_arkanoidManager.levelData != null && !EditorApplication.isPlaying)
        {
            Debug.Log(_arkanoidManager.levelData.name);
            LevelBricks = new GameObject[LevelData.LevelWidth * LevelData.LevelHeight];

            foreach (BrickPosition brickPosition in _arkanoidManager.levelData.levelBricks)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Resources/Bricks/" + brickPosition.prefabName + ".prefab", typeof(GameObject)) as GameObject;
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.transform.position = brickPosition.position;
                go.transform.parent = _arkanoidManager.Bricks;

                Vector2Int gridPosition = _grid.WorldPositionToGrid(brickPosition.position);
                LevelBricks[gridPosition.x + gridPosition.y * LevelData.LevelWidth] = go;
            }
            ChangeBackground(_arkanoidManager.levelData.backgroundSprite);
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
        if (Selection.activeTransform == _arkanoidManager.transform)
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
            Vector3 worldPosition = _grid.MousePositionToWorldPosition(mousePosition);
            Vector2Int gridPosition = _grid.WorldPositionToGrid(worldPosition);
            GameObject brickAtPosition = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            brickAtPosition.transform.parent = _arkanoidManager.Bricks;
            brickAtPosition.transform.position = worldPosition;
            LevelBricks[gridPosition.x + gridPosition.y * LevelData.LevelWidth] = brickAtPosition;
        }
    }

    private GameObject GetSceneBrick(Vector3 position)
    {
        GameObject brick = null;

        Vector2Int gridPosition = _grid.MousePositionToGridPosition(position);
        Debug.Log(position);
        Debug.Log(gridPosition);
        Debug.Log(gridPosition.x + gridPosition.y * LevelData.LevelWidth);

        if (LevelBricks[gridPosition.x + gridPosition.y * LevelData.LevelWidth] != null)
        {
            brick = LevelBricks[gridPosition.x + gridPosition.y * LevelData.LevelWidth];
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
        if (_selectedPrefab == null || _currentPrefabIndex != index)
        {
            _currentPrefabIndex = index;
            GameObject.DestroyImmediate(_selectedPrefab);
            _selectedPrefab = PrefabUtility.InstantiatePrefab(_bricksPrefabs[index]) as GameObject;
            _selectedPrefab.transform.parent = _arkanoidManager.transform;
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
        _arkanoidManager.levelData.backgroundSprite = backgroundSprite;
        _arkanoidManager.Background.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
    }

    #endregion
}