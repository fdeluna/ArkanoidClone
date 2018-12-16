using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // MODE EDIT    
    //  - Set background music mode    
    // MOVE LOGIC TO SCRIPT PAINT BRUSH REPENSARLO
    //  - CHECK IF OBJECT AT POSITION    

    private bool _edit = false;
    private LevelManager _target;
    private PaintTool _paintTool;

    private void Awake()
    {
        _target = (LevelManager)target;
        _paintTool = new PaintTool(_target);
    }

    private void OnEnable()
    {
        _edit = EditorPrefs.GetBool("_edit", false);
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;
        SceneView.onSceneGUIDelegate += HandleMouseEvents;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;
        EditorPrefs.SetBool("_edit", _edit);
        _paintTool.Reset();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (_edit)
        {
            if (GUILayout.Button("Save"))
            {
                _edit = false;
                EditorPrefs.DeleteKey("edit");
                Tools.current = Tool.View;
                SceneView.onSceneGUIDelegate -= HandleMouseEvents;
                _paintTool.Reset();
                _target.LevelData.Save(_target.LevelBricks);
            }
        }
        else
        {
            if (GUILayout.Button("Edit"))
            {
                _edit = true;
                _paintTool.Reset();
                Tools.current = Tool.None;
                SceneView.onSceneGUIDelegate += HandleMouseEvents;
                SceneView.RepaintAll();
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_edit)
        {
            _paintTool.UpdateTool();
            SceneView.RepaintAll();
        }
    }

    void HandleMouseEvents(SceneView sceneView)
    {
        // LOOK AT BACKGROUND
        // TODO FIX THIS
        //sceneView.LookAt(_target.Background.position);       
        sceneView.in2DMode = true;
        Event e = Event.current;

        _paintTool.OnMouseMove(e.mousePosition);

        if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
            _paintTool.OnMouseDown(e.mousePosition);
        }
    }
}