using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelInfo))]
public class LevelInfoEditor : Editor
{
    // MODE EDIT    
    //  - Set background music mode    
    // MOVE LOGIC TO SCRIPT PAINT BRUSH REPENSARLO
    //  - CHECK IF OBJECT AT POSITION    

    private bool _edit = false;
    private LevelInfo _target;
    private PaintTool _paintTool;

    // Serialized property
    SerializedProperty _levelDataProperty;
  
    private void OnEnable()
    {
        _target = (LevelInfo)target;
        _levelDataProperty = serializedObject.FindProperty("LevelData");
        _target.Bricks.ClearChildrens();
        _paintTool = new PaintTool(_target);

        _edit = EditorPrefs.GetBool("_edit", false);
        if (_edit)
        {
            // TODO CHANGE LOAD EDITOR TO GRID TOOL
            // REFACTOR TO GET GRID TOOL
            SceneView.onSceneGUIDelegate += HandleMouseEvents;
        }
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;
        EditorPrefs.SetBool("_edit", _edit);
        _paintTool.Reset();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_levelDataProperty);

        if (EditorGUI.EndChangeCheck() && !_edit)
        {
            serializedObject.ApplyModifiedProperties();
            _target.Bricks.ClearChildrens();
            _paintTool.LevelManager = _target;
            _paintTool.LoadEditor();
        }

        if (_edit)
        {
            if (GUILayout.Button("Clear"))
            {
                _target.Bricks.ClearChildrens();
            }

            if (GUILayout.Button("Save"))
            {
                _edit = false;
                EditorPrefs.DeleteKey("edit");
                Tools.current = Tool.View;
                SceneView.onSceneGUIDelegate -= HandleMouseEvents;
                _paintTool.Reset();
                _target.LevelData.Save(_paintTool.LevelBricks);
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
        SceneView.currentDrawingSceneView.in2DMode = true;        
        Event e = Event.current;

        if (_target.LevelData != null)
        {
            _paintTool.OnMouseMove(e.mousePosition);

            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
            {
                _paintTool.OnMouseDown(e.mousePosition);
            }
        }
    }
}