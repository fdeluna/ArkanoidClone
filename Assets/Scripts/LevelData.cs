using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Arkanoid/ New Level")]
public class LevelData : ScriptableObject
{
    public int LevelHeight = 15;
    public int LevelWidth = 10;
    public float BrickHeight = 1;
    public float BrickWidth = 0.5f;

    public Sprite background;
    public AudioClip backgroundMusic;
    public LevelData nextLevel;

    // Current Data in Inspector
    [HideInInspector]
    public GameObject[] LevelBricks
    {
        get
        {
            if (_levelBricks == null || _levelBricks.Length == 0)
            {
                _levelBricks = new GameObject[LevelWidth * LevelHeight];
            }
            return _levelBricks;
        }
    }

    // TODO MOVE TO PAINT TOOL
    private GameObject[] _levelBricks;


    // TODO POPULATE FROM PAINT TOOL
    private List<BrickPosition> _bricks = new List<BrickPosition>();

    [Serializable]
    private class BrickPosition
    {
        public Vector3 Position;
        public string PrefabName;
    }
    
    public void Save()
    {
        _bricks.Clear();
        for (int x = 0; x < LevelWidth; x++)
        {
            for (int y = 0; y < LevelHeight; y++)
            {
                if (LevelBricks[x + y * LevelWidth] != null)
                {
                    GameObject brick = LevelBricks[x + y * LevelWidth];
                    BrickPosition brickPosion = new BrickPosition
                    {
                        Position = brick.transform.position,
                        PrefabName = brick.name
                    };
                    _bricks.Add(brickPosion);
                }
            }
        }
    }

    public void LoadEditor(Transform parent)
    {
        foreach (BrickPosition brickPosition in _bricks)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Resources/Bricks/"+ brickPosition.PrefabName+".prefab", typeof(GameObject)) as GameObject;
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.transform.position = brickPosition.Position;
            go.transform.parent = parent;
            go.hideFlags = HideFlags.DontSave;
        }
    }

    public void Load(Transform parent)
    {        
        foreach (BrickPosition brickPosition in _bricks)
        {
            GameObject go = Instantiate(Resources.Load("Bricks/" + brickPosition.PrefabName)) as GameObject;
            go.transform.position = brickPosition.Position;
            go.transform.parent = parent;
            go.hideFlags = HideFlags.DontSave;
        }
    }
}