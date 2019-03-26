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
        _levelDataProperty = serializedObject.FindProperty("LevelData");
        if (_levelDataProperty.objectReferenceValue != null)
        {
            _levelDataSerializedObject = new SerializedObject(_levelDataProperty.objectReferenceValue);
            _nextLevelDataProperty = _levelDataSerializedObject.FindProperty("NextLevel");
            _powerUpChance = _levelDataSerializedObject.FindProperty("PowerUpChance");
            _powerUpsList = _levelDataSerializedObject.FindProperty("PowerUpsProbability");            
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_levelDataProperty);
        if (EditorGUI.EndChangeCheck())
        {
            _edit = false;
            _target.Bricks.ClearChildrens();
            InitEditorProperties();
            _paintTool.LoadEditor();
            
        }

        if (_levelDataProperty.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(_nextLevelDataProperty);

            if (_edit)
            {
                PowerUpsEditor();
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
                    EditorUtility.SetDirty(_target);
                    EditorUtility.SetDirty(_target.LevelData);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Edit"))
                {
                    _edit = true;
                    Tools.current = Tool.None;
                    SceneView.onSceneGUIDelegate += HandleMouseEvents;
                    SceneView.RepaintAll();
                }
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

    void PowerUpsEditor()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("PowerUps", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_powerUpChance);
        EditorGUILayout.PropertyField(_powerUpsList, true);
    }
}