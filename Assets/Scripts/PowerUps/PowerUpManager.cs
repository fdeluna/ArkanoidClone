using Level;
using UnityEngine;
using Manager;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using DG.Tweening;

namespace PowerUps
{
    public class PowerUpManager : MonoBehaviour
    {
        public int MaxPowerUps = 4;

        private List<PowerUpProbability> _powerUpsDrops;
        private List<PowerUp> _powerUps = new List<PowerUp>();
        private PowerUp _currentPowerUp;
        private float _powerUpChance = 0;

        private bool _canSpawn
        {
            get
            {
                bool canSpawn = ArkanoidManager.Instance.TotalBalls == 0;
                canSpawn &= _powerUps.Count < MaxPowerUps;
                canSpawn &= _currentPowerUp?.type != PowerUpType.EXPLOSIVE;
                canSpawn &= _currentPowerUp?.type != PowerUpType.GUN;
                canSpawn &= _currentPowerUp?.type != PowerUpType.SUPERBALL;

                return canSpawn;
            }
        }


        #region Singleton

        public static PowerUpManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<PowerUpManager>();

                if (_instance != null) return _instance;

                var goName = typeof(PowerUpManager).ToString();
                var go = GameObject.Find(goName);

                if (go != null) return _instance;

                go = new GameObject { name = goName };
                _instance = go.AddComponent<PowerUpManager>();
                return _instance;
            }
        }

        private static PowerUpManager _instance;

        #endregion

        public void Init(LevelData level)
        {
            _powerUpsDrops = new List<PowerUpProbability>();
            _powerUpChance = level.powerUpChance;
            foreach (PowerUpProbability powerUp in level.powerUpsProbability)
            {
                if (powerUp.probability > 0)
                {
                    _powerUpsDrops.Add(powerUp);
                }
            }

            if (_currentPowerUp != null)
            {
                RemoveCurrentPowerUp();
            }
        }

        public void SpawnPowerUp(Vector3 position)
        {
            if (_canSpawn)
            {
                if (!(Random.Range(0, 1f) <= _powerUpChance)) return;

                var powerUp = GetRandomPowerUp();

                if (powerUp != null)
                {
                    PoollingPrefabManager.Instance.GetPooledPrefab(powerUp, position);
                }
            }
        }

        public void CleanPowerUps()
        {
            for (int i = 0; i < _powerUps.Count; i++)
            {
                _powerUps[i].UnApplyPowerUp();
            }
        }

        // TODO REFACTOR - CHECK POWER SPAWN LOGIC
        private GameObject GetRandomPowerUp()
        {
            PowerUp powerUp = null;
            int randomPowerUp = Random.Range(0, _powerUpsDrops.Count - 1);
            powerUp = _powerUpsDrops[randomPowerUp].powerUp;

            while (powerUp == null)
            {
                randomPowerUp = Random.Range(0, _powerUpsDrops.Count - 1);
                powerUp = _powerUpsDrops[randomPowerUp].powerUp;
            }

            return powerUp.gameObject;

            // var probability = Random.Range(0, 1f);
            //
            // float totalProbability = 0;
            //

            //
            // while (powerUp == null)
            // {
            //     foreach (var pp in _powerUpsDrops)
            //     {
            //         if (pp.powerUp != null)
            //         {
            //             totalProbability += pp.probability;
            //             if (!(probability < totalProbability)) continue;
            //
            //             if (_powerUps.Count == 0)
            //             {
            //                 powerUp = pp.powerUp;
            //                 break;
            //             }
            //
            //             foreach (var pu in _powerUps)
            //             {
            //                 if (pp.powerUp != null && pu.GetType() != pp.powerUp.GetType())
            //                 {
            //                     powerUp = pp.powerUp;
            //                     break;
            //                 }
            //             }
            //         }
            //     }
            // }
            //
            // return powerUp.gameObject;
        }


        public void AddPowerUp(PowerUp powerUp)
        {
            if (_powerUps.Count < MaxPowerUps)
            {
                _powerUps.Add(powerUp);
                powerUp.transform.position = transform.position;
                UpdateUI(_powerUps.IndexOf(powerUp));
            }
            else
            {
                powerUp.gameObject.SetActive(false);
            }
        }


        private void Update()
        {
            if (ArkanoidManager.Instance.CurrentState == GameState.PLAYING)
            {
                if (_powerUps.Count > 0)
                {
                    if (_currentPowerUp != null)
                    {
                        if (!_currentPowerUp.Active)
                        {
                            _currentPowerUp.Active = true;
                            _currentPowerUp.ApplyPowerUp();
                            AudioManager.Instance.ActivePowerUp();
                        }
                        else if (_currentPowerUp.CurrentTime > 0)
                        {
                            _currentPowerUp.CurrentTime -= Time.deltaTime;
                        }
                        else
                        {
                            RemoveCurrentPowerUp();
                        }
                    }
                    else
                    {
                        _currentPowerUp = _powerUps[0];
                        _currentPowerUp.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void RemoveCurrentPowerUp()
        {
            _currentPowerUp.UnApplyPowerUp();
            _powerUps.Remove(_currentPowerUp);
            for (int i = 0; i < _powerUps.Count; i++)
            {
                UpdateUI(i);
            }

            _currentPowerUp = null;
        }

        private void UpdateUI(int index)
        {
            PowerUp pp = _powerUps[index];
            pp.transform.DOMove(transform.GetChild(index).position, 0.5f).SetEase(Ease.OutBounce);
        }
    }
}