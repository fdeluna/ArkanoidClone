using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    bool edit = false;

    // REFACTOR - On Going
    // MODE EDIT
    //  - Paint mode
    //  - Set background music mode
    //  - Set PowerUps
    // MOVE LOGIC TO SCRIPT PAINT BRUSH REPENSARLO
    //  - CHECK IF OBJECT AT POSITION
    // DRAW GRID

    private const string BRICKS_PATH = "Assets/Prefabs/Bricks";

    private LevelManager _target;
    private LevelData _levelData;
    private Vector3 _offSetPosition;

    private float _prefabPreviewWidth = 100;
    private float _prefabPreviewHeight = 90;

    private Vector2 _prefabPreviewWindowPosition;
    private List<GameObject> _bricksPrefabs = new List<GameObject>();
    private GameObject _selectedPrefab;

    private void OnEnable()
    {
        _target = (LevelManager)target;
        _levelData = _target.LevelData;
        SceneView.onSceneGUIDelegate -= GridUpdate;
        SceneView.onSceneGUIDelegate += GridUpdate;

        // Init Level Editor Data
        _bricksPrefabs = ToolsUtils.GetPrefabsAtPath(BRICKS_PATH);
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= GridUpdate;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        // PARA EDITAR
        //     HandleUtility.AddDefaultControl(
        //GUIUtility.GetControlID(FocusType.Passive));        
        DrawGrid();

        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(50f, 100f, 225f, 300f), DrawPrefabPreviewWindow, "Level Editor");
        Handles.EndGUI();
        SceneView.RepaintAll();
    }

    void DrawGrid()
    {
        _offSetPosition = new Vector3(_target.transform.position.x + _levelData.BrickWidth / 2, _target.transform.position.y - _levelData.BrickHeight / 2);
        for (int x = 0; x < _levelData.LevelWidth; x++)
        {
            for (int y = 0; y < _levelData.LevelHeight; y++)
            {
                Vector3 pos = new Vector3(_offSetPosition.x + x * _levelData.BrickWidth, _offSetPosition.y - y * _levelData.BrickHeight);
                ToolsUtils.DrawRectangle(pos, _levelData.BrickWidth, _levelData.BrickHeight, Color.clear, Color.white);
            }
        }
    }

    void GridUpdate(SceneView sceneView)
    {
        if (Selection.activeTransform == _target.transform)
        {
            Event e = Event.current;
            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePos = r.origin;

            int x = Mathf.RoundToInt(Mathf.Clamp((mousePos.x - _offSetPosition.x) / _levelData.BrickWidth, 0, _levelData.LevelWidth - 1));
            int y = Mathf.RoundToInt(Mathf.Clamp((-mousePos.y + _offSetPosition.y) / _levelData.BrickHeight, 0, _levelData.LevelHeight - 1));

            Vector3 pos = new Vector3
            {
                x = _target.transform.position.x + (x * _levelData.BrickWidth + _levelData.BrickWidth / 2.0f),
                y = _target.transform.position.y - (y * _levelData.BrickHeight + _levelData.BrickHeight / 2.0f)
            };

            Handles.color = Color.green;
            Handles.DrawWireCube(pos, new Vector3(_levelData.BrickWidth, _levelData.BrickHeight));
            if (_selectedPrefab != null)
            {

                _selectedPrefab.transform.position = pos;
            }
        }
    }

    private void DrawPrefabPreviewWindow(int windowID)
    {
        _prefabPreviewWindowPosition = GUILayout.BeginScrollView(_prefabPreviewWindowPosition);
        int selectionIndex = -1;
        selectionIndex = GUILayout.SelectionGrid(
            selectionIndex,
            GetGUIContentsFromPrefabs(_bricksPrefabs),
            2,
            GetGUIStyle()
            );
        GUILayout.EndScrollView();
        GetSelectedItem(selectionIndex, _bricksPrefabs);
    }

    private GUIContent[] GetGUIContentsFromPrefabs(List<GameObject> prefabs)
    {
        List<GUIContent> guiContents = new List<GUIContent>();
        if (prefabs.Count > 0)
        {
            foreach (GameObject prefab in prefabs)
            {
                GUIContent guiContent = new GUIContent();
                guiContent.text = prefab.name;
                guiContent.image = AssetPreview.GetAssetPreview(prefab);
                guiContents.Add(guiContent);
            }
        }
        return guiContents.ToArray();
    }


    private GUIStyle GetGUIStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.imagePosition = ImagePosition.ImageAbove;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.fixedWidth = _prefabPreviewWidth;
        guiStyle.fixedHeight = _prefabPreviewHeight;
        return guiStyle;
    }

    private void GetSelectedItem(int index, List<GameObject> prefabs)
    {
        if (index != -1)
        {
            DestroyImmediate(_selectedPrefab);
            _selectedPrefab = PrefabUtility.InstantiatePrefab(prefabs[index]) as GameObject;
            _selectedPrefab.transform.parent = _target.transform;
        }
    }
}