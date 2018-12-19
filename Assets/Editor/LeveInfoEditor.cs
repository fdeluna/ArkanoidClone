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

    private void Awake()
    {
        _target = (LevelInfo)target;
        _paintTool = new PaintTool(_target);
        _levelDataProperty = serializedObject.FindProperty("LevelData");
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
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_levelDataProperty);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            ClearChildren();
            _paintTool.LoadEditor();
        }
        //EditorGUI.EndChangeCheck();

        if (_edit)
        {
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
        // LOOK AT BACKGROUND        
        //sceneView.LookAt(_target.Background.position);       
        sceneView.in2DMode = true;
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

    public void ClearChildren()
    {
        //Array to hold all child obj
        List<Transform> allChildren = new List<Transform>();

        //Find all child obj and store to that array
        foreach (Transform child in _target.Bricks)
        {
            allChildren.Add(child);
        }

        //Now destroy them
        foreach (Transform child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}