using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // REFACTOR - On Going
    // MODE EDIT
    //  - Paint mode
    //  - Set background music mode
    //  - Set PowerUps
    // MOVE LOGIC TO SCRIPT PAINT BRUSH REPENSARLO
    //  - CHECK IF OBJECT AT POSITION
    // DRAW GRID

    bool edit = false;

    private LevelManager _target;
    private PaintTool _paintTool;

    private void OnEnable()
    {
        _target = (LevelManager)target;
        _paintTool = new PaintTool(_target);
    }

    private void OnDisable()
    {        
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;        
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (edit)
        {
            if (GUILayout.Button("Save"))
            {
                edit = false;
                //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
                Tools.current = Tool.View;
                SceneView.onSceneGUIDelegate -= HandleMouseEvents;
                // TODO SAVE IN SCRIPTABLE OBJECT
                Debug.Log("Save in ScriptableObject");
            }
        }
        else
        {
            if (GUILayout.Button("Edit"))
            {
                edit = true;
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
        Event e = Event.current;

        _paintTool.OnMouseMove(e.mousePosition);

        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            _paintTool.OnMouseDown(e.mousePosition);
        }
    }
}