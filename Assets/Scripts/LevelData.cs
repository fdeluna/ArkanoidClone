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
      
    // TODO POPULATE FROM PAINT TOOL
    public List<BrickPosition> LevelBricks = new List<BrickPosition>();

    [Serializable]
    public class BrickPosition
    {
        public Vector3 Position;
        public string PrefabName;
    }
    
    public void Save(GameObject[] LevelBricks)
    {
        this.LevelBricks.Clear();
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
                    this.LevelBricks.Add(brickPosion);
                }
            }
        }
    }

    public void Load(Transform parent)
    {        
        foreach (BrickPosition brickPosition in LevelBricks)
        {
            GameObject go = Instantiate(Resources.Load("Bricks/" + brickPosition.PrefabName)) as GameObject;
            go.transform.position = brickPosition.Position;
            go.transform.parent = parent;
            go.hideFlags = HideFlags.DontSave;
        }
    }
}