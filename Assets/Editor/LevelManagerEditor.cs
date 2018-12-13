using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // MODE EDIT
    //  - Paint mode
    //  - Delete mode
    //  - Set background music mode    
    // MOVE LOGIC TO SCRIPT PAINT BRUSH REPENSARLO
    //  - CHECK IF OBJECT AT POSITION    

    bool edit = false;
    private LevelManager _target;
    private PaintTool _paintTool;
    
    private void Awake()
    {
        _target = (LevelManager)target;
        _paintTool = new PaintTool(_target);        
    }

    private void OnEnable()
    {
        edit = EditorPrefs.GetBool("edit", false);
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;
        SceneView.onSceneGUIDelegate += HandleMouseEvents;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;
        EditorPrefs.SetBool("edit", edit);
        _paintTool.Reset();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (edit)
        {
            if (GUILayout.Button("Save"))
            {
                edit = false;
                EditorPrefs.DeleteAll();
                Tools.current = Tool.View;
                SceneView.onSceneGUIDelegate -= HandleMouseEvents;
                _paintTool.Reset();
                Debug.Log("Save in ScriptableObject");
            }
        }
        else
        {
            if (GUILayout.Button("Edit"))
            {
                edit = true;
                _paintTool.Reset();
                Tools.current = Tool.None;
                SceneView.onSceneGUIDelegate += HandleMouseEvents;
                SceneView.RepaintAll();
            }
        }
    }

    private void OnSceneGUI()
    {
        if (edit)
        {
            _paintTool.UpdateTool();
            SceneView.RepaintAll();
        }
    }

    void HandleMouseEvents(SceneView sceneView)
    {
        // LOOK AT BACKGROUND
        sceneView.LookAt(_target.Background.position);
        sceneView.in2DMode = true;
        Event e = Event.current;

        _paintTool.OnMouseMove(e.mousePosition);

        if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
            _paintTool.OnMouseDown(e.mousePosition);
        }
    }
}