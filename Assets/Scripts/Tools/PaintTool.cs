using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PaintTool : LevelEditorTool
{

    private const string BRICKS_PATH = "Assets/Prefabs/Bricks";

    private Vector3 _paleteWindowPosition;
    private List<GameObject> _bricksPrefabs = new List<GameObject>();
    private GameObject _selectedPrefab;


    protected override void Init(LevelManager levelmanager)
    {
        base.Init(levelmanager);
        _bricksPrefabs = ToolsUtils.GetPrefabsAtPath(BRICKS_PATH);
    }

    protected override void DrawTool()
    {
        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(50f, 100f, 200f, 360f), DrawPrefabPreviewWindow, "Level Editor");
        Handles.EndGUI();        
    }

    protected override void OnMouseDown(Vector3 mousePosition)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseMove(Vector3 mousePosition)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseUp()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateTool()
    {
        throw new System.NotImplementedException();
    }

    private void DrawPrefabPreviewWindow(int windowID)
    {
        _paleteWindowPosition = GUILayout.BeginScrollView(_paleteWindowPosition);
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
        guiStyle.imagePosition = ImagePosition.ImageOnly;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.fixedWidth = _prefabPreviewWidth;
        guiStyle.fixedHeight = _prefabPreviewHeight;
        return guiStyle;
    }

    private void GetSelectedItem(int index, List<GameObject> prefabs)
    {
        if (index != -1)
        {
            GameObject.DestroyImmediate(_selectedPrefab);
            _selectedPrefab = PrefabUtility.InstantiatePrefab(prefabs[index]) as GameObject;
            _selectedPrefab.transform.parent = _levelManager.transform;
        }
    }
}
