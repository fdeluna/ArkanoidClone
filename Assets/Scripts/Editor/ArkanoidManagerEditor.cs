using Level;
using Manager;
using PowerUps;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [ExecuteInEditMode]
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
            InitEditorProperties();
            InitPowerUps();

            _edit = EditorPrefs.GetBool("_edit", false);
            if (_edit)
            {
                SceneView.onSceneGUIDelegate += HandleMouseEvents;
            }
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= HandleMouseEvents;
            _paintTool.Reset();
            EditorPrefs.SetBool("_edit", _edit);
        }

        private void InitEditorProperties()
        {
            _target = (ArkanoidManager)target;
            _paintTool = new LevelEditorTool(_target);

            _levelDataProperty = serializedObject.FindProperty("levelData");
            serializedObject.ApplyModifiedProperties();

            if (_levelDataProperty.objectReferenceValue == null) return;

            _levelDataSerializedObject = new SerializedObject(_levelDataProperty.objectReferenceValue);
            _nextLevelDataProperty = _levelDataSerializedObject.FindProperty("nextLevel");
            _powerUpChance = _levelDataSerializedObject.FindProperty("powerUpChance");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_levelDataProperty);
            if (EditorGUI.EndChangeCheck())
            {
                _edit = false;
                InitEditorProperties();
                _paintTool.LoadEditor();
            }

            if (_levelDataProperty.objectReferenceValue == null) return;

            EditorGUILayout.PropertyField(_nextLevelDataProperty);

            if (Application.isPlaying && _edit)
            {
                _edit = false;
                _paintTool.Reset();
                SceneView.onSceneGUIDelegate -= HandleMouseEvents;
                SceneView.RepaintAll();
            }
            else
            {
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
                    _levelDataSerializedObject.ApplyModifiedProperties();

                    EditorUtility.SetDirty(_target);
                    EditorUtility.SetDirty(_target.levelData);
                    AssetDatabase.SaveAssets();
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
        }

        private void OnSceneGUI()
        {
            if (!_edit || Application.isPlaying) return;

            _paintTool.UpdateTool();
            SceneView.RepaintAll();
        }

        private void HandleMouseEvents(SceneView sceneView)
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

        #region Power Ups

        private void InitPowerUps()
        {
            var powerUps = Utils.GetPrefabsAtPath<PowerUp>(Utils.PowerUpsPath);
            var initProbability = 1f / powerUps.Count;

            foreach (var powerUpPrefab in powerUps)
            {
                if (_target.levelData.powerUpsProbability.Count == 0 ||
                    !_target.levelData.powerUpsProbability.Any(pp => pp.powerUp == powerUpPrefab))
                {
                    _target.levelData.powerUpsProbability.Add(new PowerUpProbability()
                        { powerUp = powerUpPrefab, probability = initProbability });
                }
            }

            EditorUtility.SetDirty(_target);
            EditorUtility.SetDirty(_target.levelData);
            AssetDatabase.SaveAssets();
        }

        private void PowerUpsEditor()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("PowerUps", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_powerUpChance);

            foreach (var pp in _target.levelData.powerUpsProbability)
            {
                if (pp != null && pp.powerUp != null)
                {
                    EditorGUI.BeginChangeCheck();

                    float initProbability = pp.probability;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(pp.powerUp.type.ToString());

                    pp.probability = EditorGUILayout.Slider(pp.probability, 0, 1);

                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck() && initProbability != pp.probability)
                    {
                        // proportion to apply
                        // look info for proportions
                        float ratio = (1 - pp.probability) / (1 - initProbability);
                        RedistributeProbabilities(pp, ratio);
                        break;
                    }
                }
            }
        }

        private void RedistributeProbabilities(PowerUpProbability powerUpProbability, float ratio)
        {
            foreach (var pp in _target.levelData.powerUpsProbability)
            {
                if (powerUpProbability != pp && pp.probability != 0)
                {
                    pp.probability *= ratio;
                }
            }
        }

        #endregion
    }
}