using System;
using System.Collections.Generic;
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

    private List<Transform> _bricks;

    // TODO SAVE PREFAB AND LIST MAP???
    public void Save(GameObject[] levelBricks)
    {
        //_bricks.Clear();
        //foreach (Transform t in level)
        //{
        //    _bricks.Add(t);
        //}



    }

    public void Load(LevelManager level)
    {
        // TODO BACKGROUND
        // TODO BACKGROUND AUDIO

        foreach (Transform t in _bricks)
        {
            GameObject go = Instantiate(t.gameObject, level.Bricks);
            go.transform.position = t.position;
            go.transform.parent = level.Bricks;
        }
    }
}