using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // START WITH POWER UPS SYSTEM          
    private bool _edit = false;
    private LevelManager _target;
    private LevelEditorTool _paintTool;

    // Serialized property
    SerializedProperty _levelDataProperty;
    SerializedProperty _powerUpChance;
    SerializedProperty _nextLevelDataProperty;
    SerializedProperty _powerUpsList;

    // Serialized object
    SerializedObject _levelDataSerializedObject;

    private void OnEnable()
    {
        _target = (LevelManager)target;
        InitEditorProperties();
        _target.Bricks.ClearChildrens();
        _paintTool = new LevelEditorTool(_target);

        _edit = EditorPrefs.GetBool("_edit", false);
        if (_edit)
        {
            SceneView.onSceneGUIDelegate += HandleMouseEvents;
        }
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= HandleMouseEvents;
        EditorPrefs.SetBool("_edit", _edit);
        _paintTool.Reset();
    }

    private void InitEditorProperties()
    {
        // properties
        _levelDataProperty = serializedObject.FindProperty("LevelData");
        _levelDataSerializedObject = new SerializedObject(_levelDataProperty.objectReferenceValue);
        _nextLevelDataProperty = _levelDataSerializedObject.FindProperty("NextLevel");
        _powerUpChance = _levelDataSerializedObject.FindProperty("PowerUpChance");
        _powerUpsList = _levelDataSerializedObject.FindProperty("PowerUpsProbability");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_levelDataProperty);
        EditorGUILayout.PropertyField(_nextLevelDataProperty);

        if (EditorGUI.EndChangeCheck())
        {
            _levelDataSerializedObject.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
            _edit = false;
            _target.Bricks.ClearChildrens();            
            _paintTool.LoadEditor();            
            InitEditorProperties();
        }

        if (_edit)
        {
            PowerUpsEditor();
            if (GUILayout.Button("Clear"))
            {
                _target.Bricks.ClearChildrens();
            }

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(_target);
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
        _levelDataSerializedObject.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
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

    void PowerUpsEditor()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("PowerUps", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_powerUpChance);
        EditorGUILayout.PropertyField(_powerUpsList, true);
    }
}