using Manager;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ArkanoidManager))]
    public class ArkanoidManagerEditor : UnityEditor.Editor
    {
        // START WITH POWER UPS SYSTEM          
        private bool _edit = false;
        private ArkanoidManager _target;
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
            _target = (ArkanoidManager)target;
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
        }

        private void InitEditorProperties()
        {
            _levelDataProperty = serializedObject.FindProperty("levelData");
            if (_levelDataProperty.objectReferenceValue == null) return;
            
            _levelDataSerializedObject = new SerializedObject(_levelDataProperty.objectReferenceValue);
            _nextLevelDataProperty = _levelDataSerializedObject.FindProperty("nextLevel");
            _powerUpChance = _levelDataSerializedObject.FindProperty("powerUpChance");
            _powerUpsList = _levelDataSerializedObject.FindProperty("powerUpsProbability");            
            serializedObject.ApplyModifiedProperties();
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

            if (_levelDataProperty.objectReferenceValue == null) return;
            EditorGUILayout.PropertyField(_nextLevelDataProperty);

            if (_edit)
            {
                PowerUpsEditor();
                if (GUILayout.Button("Clear"))
                {
                    _target.Bricks.ClearChildrens();
                }

                if (!GUILayout.Button("Save")) return;
                    
                _edit = false;
                EditorPrefs.DeleteKey("edit");
                Tools.current = Tool.View;
                SceneView.onSceneGUIDelegate -= HandleMouseEvents;
                _paintTool.Reset();
                _target.levelData.Save(_paintTool.LevelBricks);
                EditorUtility.SetDirty(_target);
                EditorUtility.SetDirty(_target.levelData);
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (!GUILayout.Button("Edit")) return;
                    
                _edit = true;
                Tools.current = Tool.None;
                SceneView.onSceneGUIDelegate += HandleMouseEvents;
                _paintTool.Reset();
                SceneView.RepaintAll();
            }
        }

        private void OnSceneGUI()
        {
            if (!_edit) return;
            
            _paintTool.UpdateTool();
            SceneView.RepaintAll();
        }

        void HandleMouseEvents(SceneView sceneView)
        {
            SceneView.currentDrawingSceneView.in2DMode = true;
            var e = Event.current;

            if (_target.levelData == null) return;
            
            _paintTool.OnMouseMove(e.mousePosition);

            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
            {
                _paintTool.OnMouseDown(e.mousePosition);
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
}