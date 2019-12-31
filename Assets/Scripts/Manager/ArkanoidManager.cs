using System.Collections.Generic;
using System.Linq;
using Controller;
using Level;
using PowerUps;
using UnityEngine;

namespace Manager
{
    [DisallowMultipleComponent]
    public class ArkanoidManager : ArkanoidObject
    {
        #region Level Data
        public LevelData levelData;
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

        #region Current Game
        [HideInInspector]
        public int totalBalls = 1;
        private List<PowerUpProbability> _levelPowerUps;
        private PaddleController _paddle;
        #endregion

        private void Start()
        {
            _paddle = FindObjectOfType<PaddleController>();
            CleanLevel();
        }

        private void LoadLevel()
        {
            CleanLevel();
            totalBricks = levelData.Load(this, () => GameManager.Instance.CurrentState = GameManager.GameState.Playing);
            _levelPowerUps = levelData.powerUpsProbability.Where(o => o.probability > 0).ToList();
        }

        private void CleanLevel()
        {
            LevelData.Clean(this);
        }

        private void LoadNextLevel()
        {
            levelData = levelData.nextLevel;
            if (levelData != null)
            {
                LoadLevel();
            }
        }

        public void OnBrickDestroyed(Brick brick)
        {
            totalBricks--;
            SpawnPowerUp(brick.transform.position);
            if (totalBricks > 0) return;
            Debug.Log("WIN");
            LoadNextLevel();
        }

        public void OnBallDestroyed()
        {
            totalBalls--;
            if (totalBalls > 0) return;
            _paddle.lives--;
            GameManager.Instance.CurrentState = _paddle.lives < 0 ? GameManager.GameState.GameOver : GameManager.GameState.PlayerDead;
        }

        private void SpawnPowerUp(Vector3 position)
        {
            if (totalBalls != 1) return;
            if (!(Random.Range(0, 1f) <= levelData.powerUpChance)) return;
            
            var powerUp = GetRandomPowerUp();
            while (powerUp == null)
            {
                powerUp = GetRandomPowerUp();
            }
            PoollingPrefabManager.Instance.GetPooledPrefab(powerUp, position);
        }

        private GameObject GetRandomPowerUp()
        {
            GameObject powerUp = null;
            var probability = Random.Range(0, 1f);

            float totalProbability = 0;
            foreach (var pp in _levelPowerUps)
            {
                totalProbability += pp.probability;
                if (!(probability < totalProbability)) continue;
                if (_paddle.currentPowerUp?.GetType() != pp.powerUp.GetComponent<PowerUp>().GetType() && _levelPowerUps.Count > 1)
                {
                    powerUp = pp.powerUp;
                    break;
                }
            }
            return powerUp;
        }

        protected override void OnGameStateChanged(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.LoadGame:
                    gameObject.SetActive(true);
                    LoadLevel();
                    break;
                case GameManager.GameState.GameOver:                
                    CleanLevel();
                    break;
            }
        }
    }
}