using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class LevelData : ScriptableObject
    {
        private enum SpawnAnimation { TopDown, LeftRight, Scale };

        public const int LevelHeight = 14;
        public const int LevelWidth = 16;
        public const float BrickHeight = 0.5f;
        public const float BrickWidth = 1;

        public Sprite backgroundSprite;
        public AudioClip backgroundMusic;
        public LevelData nextLevel;
        public List<BrickPosition> levelBricks = new List<BrickPosition>();

        [Range(0, 1)]
        public float powerUpChance = 0.1f;
        public List<PowerUpProbability> powerUpsProbability = new List<PowerUpProbability>();

        public void Save(GameObject[] levelBricks)
        {
            this.levelBricks.Clear();
            for (var x = 0; x < LevelWidth; x++)
            {
                for (var y = 0; y < LevelHeight; y++)
                {
                    if (levelBricks[x + y * LevelWidth] == null) continue;
                    var brick = levelBricks[x + y * LevelWidth];
                    var brickPosition = new BrickPosition
                    {
                        position = brick.transform.position,
                        prefabName = brick.name
                    };
                    this.levelBricks.Add(brickPosition);
                }
            }
        }

        public int Load(ArkanoidManager levelManager, System.Action callBack = null)
        {
            var totalBricks = 0;
            var spawnAnimation = (SpawnAnimation)Random.Range(0, 3);
            var sequence = DOTween.Sequence();
            foreach (var brickPosition in levelBricks)
            {
                var brick = ((GameObject) Instantiate(Resources.Load("Bricks/" + brickPosition.prefabName))).GetComponent<Brick>();
                brick.transform.parent = levelManager.Bricks;
                brick.transform.position = brickPosition.position;
                Tween brickTween = null;
                
                switch (spawnAnimation)
                {
                    case SpawnAnimation.TopDown:
                        brickTween = SpawnTopDown(brick.transform, brickPosition.position);
                        break;
                    case SpawnAnimation.LeftRight:
                        brickTween = SpawnLeftRight(brick.transform, brickPosition.position);
                        break;
                    case SpawnAnimation.Scale:
                        brickTween = SpawnScale(brick.transform);
                        break;
                }
                
                sequence.Insert(0,brickTween);
                brick.OnBrickDestroyed += levelManager.OnBrickDestroyed;
                totalBricks = brick.brickType == Brick.BrickType.Destructible ? totalBricks + 1 : totalBricks;
            }
            sequence.OnComplete(() => callBack?.Invoke());
            return totalBricks;
        }


        private Tween SpawnTopDown(Transform brick, Vector3 endPosition)
        {
            var startPosition = new Vector3(brick.transform.localPosition.x, 1);
            brick.transform.localPosition = startPosition;
            return brick.transform.DOMove(endPosition, 2f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutExpo).OnComplete(() => brick.transform.position = endPosition);
        }

        private Tween SpawnLeftRight(Transform brick, Vector3 endPosition)
        {
            float x = brick.position.x > 0 ? 10 : -10;
            var startPosition = new Vector3(x, brick.transform.position.y);
            brick.transform.position = startPosition;
            return brick.transform.DOMove(endPosition, 1.5f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutExpo).OnComplete(() => brick.transform.position = endPosition);
        }

        private Tween SpawnScale(Transform brick)
        {
            var endScale = brick.localScale;
            brick.transform.localScale = Vector3.zero;
            return brick.transform.DOScale(endScale, 1.5f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutElastic).OnComplete(() => brick.transform.localScale = endScale);
        }


        public static void Clean(ArkanoidManager levelManager)
        {
            foreach (Transform t in levelManager.Bricks)
            {
                var brick = t.GetComponent<Brick>();
                brick.OnBrickDestroyed -= levelManager.OnBrickDestroyed;
                Destroy(brick.gameObject);
            }
        }

        [MenuItem("Arkanoid/ New Level")]
        public static void CreateLevel()
        {
            var newLevel = ScriptableObject.CreateInstance<LevelData>();
            // TODO GET LAST ScriptableObject NAME        
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/new Level.asset");
            AssetDatabase.CreateAsset(newLevel, assetPathAndName);
            var powerUps = Utils.GetPrefabsAtPath(Utils.PowerUpsPath);

            foreach (var powerUpPrefab in powerUps)
            {
                newLevel.powerUpsProbability.Add(new PowerUpProbability() { powerUp = powerUpPrefab });
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}