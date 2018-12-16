using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
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

    private List<BrickPosition> _bricks;

    [Serializable]
    private class BrickPosition
    {
        public Vector3 Position;
        public string PrefabName;
    }

    // TODO SAVE PREFAB AND LIST MAP???
    public void Save(GameObject[] levelBricks)
    {
        _bricks.Clear();
        for (int x = 0; x < LevelWidth; x++)
        {
            for (int y = 0; y < LevelHeight; y++)
            {
                if (levelBricks[x + y * LevelWidth] != null)
                {
                    GameObject brick = levelBricks[x + y * LevelWidth];
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

    public void Load(LevelInfo level)
    {
        // TODO BACKGROUND
        // TODO BACKGROUND AUDIO        
        //GameObject go = Resources.Load<GameObject>("Assets/Prefabs/Resources/Bricks/Blue Brick.prefab") as GameObject;

        foreach (BrickPosition brickPosition in _bricks)
        {
            GameObject go = Instantiate(Resources.Load("Bricks/" + brickPosition.PrefabName)) as GameObject;
            go.transform.position = brickPosition.Position;
            go.transform.parent = level.Bricks;
            go.hideFlags = HideFlags.DontSave;
        }
    }
}