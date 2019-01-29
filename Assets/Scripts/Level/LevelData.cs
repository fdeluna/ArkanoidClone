using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Arkanoid/ New Level")]
public class LevelData : ScriptableObject
{
    public readonly int LevelHeight = 14;
    public readonly int LevelWidth = 16;
    public readonly float BrickHeight = 0.5f;
    public readonly float BrickWidth = 1;

    public Material Background;
    public AudioClip BackgroundMusic;
    public LevelData NextLevel;
    [Range(0, 1)]
    public float PowerUpChance = 0.1f;

    [HideInInspector]
    public List<BrickPosition> LevelBricks = new List<BrickPosition>();

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
        AssetDatabase.SaveAssets();
    }

    public int Load(LevelManager levelManager)
    {
        int totalBricks = 0;
        foreach (BrickPosition brickPosition in LevelBricks)
        {
            Brick brick = (Instantiate(Resources.Load("Bricks/" + brickPosition.PrefabName)) as GameObject).GetComponent<Brick>();
            brick.transform.position = brickPosition.Position;
            brick.transform.parent = levelManager.Bricks;
            brick.OnBrickDestroyed += levelManager.OnBrickDestroyed;
            totalBricks = brick.bricktype == Brick.BrickType.DESTRUCTIBLE ? totalBricks + 1 : totalBricks;
            brick.hideFlags = HideFlags.DontSave;
        }

        return totalBricks;
    }

    public void Clean(LevelManager levelManager)
    {
        foreach (Transform t in levelManager.Bricks)
        {
            Brick brick = t.GetComponent<Brick>();
            brick.OnBrickDestroyed -= levelManager.OnBrickDestroyed;
            Destroy(brick.gameObject);
        }
    }
}


[Serializable]
public class BrickPosition
{
    public Vector3 Position;
    public string PrefabName;
}