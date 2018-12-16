﻿using UnityEngine;

[ExecuteInEditMode,DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{
    public LevelData LevelData;

    public Transform Background
    {
        get
        {
            if (_background == null)
            {
                _background = transform.Find("Background");
            }
            return _background;
        }
    }
    private Transform _background;

    public Transform Bricks
    {
        get
        {
            if (_bricks == null)
            {
                _bricks = transform.Find("Bricks");
            }
            return _bricks;
        }
    }

    public GameObject[] LevelBricks;

    private Transform _bricks;


    // AQUI FALTA MAPEAR LAS VARIABLES DEL SCRIPTABLEoBJECT

    void Awake()
    {
        CleanLevel();
        LevelBricks = new GameObject[LevelData.LevelWidth * LevelData.LevelHeight];
    }

    private void Start()
    {
        LevelData.Load(this);
    }

    void CleanLevel()
    {
        if (Bricks.childCount > 0)
        {
            foreach (Transform t in Bricks)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
    }
    
}
