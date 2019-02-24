﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{
    #region Level Data
    public LevelData LevelData;
    static int totalBricks;
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
    private Transform _bricks;

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
    #endregion

    #region Powerups
    private List<PowerUpProbability> _levelPowerUps;
    private PaddleController _paddle;
    #endregion


    private void Awake()
    {
        _paddle = FindObjectOfType<PaddleController>();
    }

    public void LoadLevel()
    {
        CleanLevel();
        totalBricks = LevelData.Load(this);
        _levelPowerUps = LevelData.PowerUpsProbability.OrderBy(o => o.probability).ToList();
    }

    public void CleanLevel()
    {
        LevelData.Clean(this);
    }

    public void LoadNextLevel()
    {
        LevelData = LevelData.NextLevel;
        LoadLevel();
    }

    public void OnBrickDestroyed(Brick brick)
    {
        totalBricks--;
        SpawnPowerUp(brick.transform.position);
        if (totalBricks <= 0)
        {
            Debug.Log("WIN");
            LoadNextLevel();
        }
    }

    private void SpawnPowerUp(Vector3 position)
    {
        if (Random.Range(0, 1f) <= LevelData.PowerUpChance)
        {
            GameObject powerUp = GetRandomPowerUp();
            while (powerUp == null)
            {
                powerUp = GetRandomPowerUp();
            }
            PoollingPrefabManager.Instance.GetPooledPrefab(powerUp, position);
        }
    }

    private GameObject GetRandomPowerUp()
    {
        GameObject powerUp = null;
        float probability = Random.Range(0, 1f);

        float totalProbability = 0;
        foreach (PowerUpProbability pp in _levelPowerUps)
        {
            totalProbability += pp.probability;
            if (probability < totalProbability)
            {
                if (_paddle.CurrentPowerUp?.GetType() != pp.powerUp.GetComponent<PowerUp>().GetType())
                {
                    powerUp = pp.powerUp;
                    break;
                }
            }
        }
        return powerUp;
    }
}