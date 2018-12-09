﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PaintTool
{
    private const string BRICKS_PATH = "Assets/Prefabs/Bricks";

    private LevelManager _levelManager;
    private Vector3 _offSetPosition;

    private Vector3 _paleteWindowPosition;
    private int _selectedPrefabIndex = -1;
    private int _currentPrefabIndex = -1;
    private float _prefabPreviewWidth = 100f;
    private float _prefabPreviewHeight = 100f;


    private List<GameObject> _bricksPrefabs = new List<GameObject>();
    private GameObject _selectedPrefab;

    public PaintTool(LevelManager levelManager)
    {
        _levelManager = levelManager;
        _bricksPrefabs = ToolsUtils.GetPrefabsAtPath(BRICKS_PATH);
    }

    public void Reset()
    {
        GameObject.DestroyImmediate(_selectedPrefab);
    }

    public void UpdateTool()
    {
        DrawGrid();
        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(50f, 100f, 225f, 360f), DrawPrefabPreviewWindow, "Level Editor");
        Handles.EndGUI();
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }

    public void OnMouseMove(Vector3 mousePosition)
    {
        if (Selection.activeTransform == _levelManager.transform)
        {
            if (_selectedPrefab != null)
            {
                _selectedPrefab.transform.position = WorldPositionToGrid(MousePositionToWorldPosition(mousePosition));
            }
        }
    }

    public void OnMouseDown(Vector3 mousePosition)
    {
        CreateBrick(_selectedPrefab, WorldPositionToGrid(MousePositionToWorldPosition(mousePosition)));
    }

    private Vector3 MousePositionToWorldPosition(Vector3 mousePosition)
    {
        Camera camera = SceneView.currentDrawingSceneView.camera;
        mousePosition.y = camera.pixelHeight - mousePosition.y;
        return camera.ScreenToWorldPoint(mousePosition);
    }

    private Vector3 WorldPositionToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((worldPosition.x - _offSetPosition.x) / _levelManager.LevelData.BrickWidth, 0, _levelManager.LevelData.LevelWidth - 1));
        int y = Mathf.RoundToInt(Mathf.Clamp((-worldPosition.y + _offSetPosition.y) / _levelManager.LevelData.BrickHeight, 0, _levelManager.LevelData.LevelHeight - 1));

        Vector3 pos = new Vector3
        {
            x = _levelManager.transform.position.x + (x * _levelManager.LevelData.BrickWidth + _levelManager.LevelData.BrickWidth / 2.0f),
            y = _levelManager.transform.position.y - (y * _levelManager.LevelData.BrickHeight + _levelManager.LevelData.BrickHeight / 2.0f)
        };

        return pos;
    }

    private void DrawGrid()
    {
        _offSetPosition = new Vector3(_levelManager.transform.position.x + _levelManager.LevelData.BrickWidth / 2, _levelManager.transform.position.y - _levelManager.LevelData.BrickHeight / 2);
        for (int x = 0; x < _levelManager.LevelData.LevelWidth; x++)
        {
            for (int y = 0; y < _levelManager.LevelData.LevelHeight; y++)
            {
                Vector3 pos = new Vector3(_offSetPosition.x + x * _levelManager.LevelData.BrickWidth, _offSetPosition.y - y * _levelManager.LevelData.BrickHeight);
                ToolsUtils.DrawRectangle(pos, _levelManager.LevelData.BrickWidth, _levelManager.LevelData.BrickHeight, Color.clear, Color.white);
            }
        }
    }

    private void DrawPrefabPreviewWindow(int windowID)
    {
        _paleteWindowPosition = GUILayout.BeginScrollView(_paleteWindowPosition);
        _selectedPrefabIndex = GUILayout.SelectionGrid(
            _selectedPrefabIndex,
            GetGUIContentsFromPrefabs(_bricksPrefabs),
            2,
            GetGUIStyle()
            );
        GUILayout.EndScrollView();
        GetSelectedItem(_selectedPrefabIndex);
    }

    private void GetSelectedItem(int index)
    {
        if (index != -1 && _currentPrefabIndex != index)
        {
            _currentPrefabIndex = index;
            GameObject.DestroyImmediate(_selectedPrefab);
            _selectedPrefab = PrefabUtility.InstantiatePrefab(_bricksPrefabs[index]) as GameObject;
            _selectedPrefab.transform.parent = _levelManager.transform;
        }
    }

    private void CreateBrick(GameObject brick, Vector3 position)
    {
        GameObject go = GameObject.Instantiate(brick);
        go.transform.parent = _levelManager.transform;
        go.transform.position = position;
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
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button)
        {
            imagePosition = ImagePosition.ImageAbove,
            alignment = TextAnchor.UpperCenter,
            fixedWidth = _prefabPreviewWidth,
            fixedHeight = _prefabPreviewHeight
        };
        return guiStyle;
    }
}
