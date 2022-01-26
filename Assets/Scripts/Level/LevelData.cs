using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Level
{
    public class LevelData : ScriptableObject
    {
        private enum SpawnAnimation { TopDown, LeftRight, Scale };

        public const int LevelHeight = 12;
        public const int LevelWidth = 12;
        public const float BrickHeight = 0.5f;
        public const float BrickWidth = 1f;

        public string levelName;
        public Sprite backgroundSprite;
        public AudioClip backgroundMusic;
        public LevelData nextLevel;
        public List<BrickPosition> levelBricks = new List<BrickPosition>();

        [Range(0, 1)]
        public float powerUpChance = 0.1f;
        [HideInInspector]
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
            LoadBackground(levelManager);
            int totalBricks = LoadBricks(levelManager, callBack);
            return totalBricks;
        }


        private void LoadBackground(ArkanoidManager levelManager)
        {
            bool front = Random.value % 2 == 0;

            int currentSortingOrder = levelManager.Background.GetComponent<SortingGroup>().sortingOrder;


            GameObject newBackground = Instantiate(levelManager.Background.gameObject, levelManager.Background.position, Quaternion.identity, levelManager.transform);
            newBackground.name = "Background";


            SpriteRenderer newBackgroundSprite = newBackground.GetComponent<SpriteRenderer>();
            newBackgroundSprite.sprite = backgroundSprite;

            SortingGroup newBackgroundSG = newBackground.GetComponent<SortingGroup>();
            newBackgroundSG.sortingOrder++;

            Transform mask = newBackgroundSprite.transform.Find("Mask");
            Vector3 maskScale = front ? Vector3.zero : mask.localScale;


            if (front)
            {
                newBackgroundSprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                newBackgroundSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                mask.localScale = Vector3.zero;
            }


            mask.DOScale(maskScale, 3f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                Destroy(levelManager.Background.gameObject);
                newBackgroundSG.sortingOrder = currentSortingOrder;
                levelManager.Background = newBackground.transform;
            });
        }

        #region LoadBricks

        private int LoadBricks(ArkanoidManager levelManager, Action callBack)
        {
            var totalBricks = 0;
            var spawnAnimation = (SpawnAnimation)Random.Range(0, 3);
            var sequence = DOTween.Sequence();
            foreach (var brickPosition in levelBricks)
            {
                var brick = ((GameObject)Instantiate(Resources.Load("Bricks/" + brickPosition.prefabName))).GetComponent<Brick>();
                brick.transform.parent = levelManager.Bricks;
                brick.transform.position = brickPosition.position;
                Tween brickTween = null;

                switch (spawnAnimation)
                {
                    case SpawnAnimation.TopDown:
                        brickTween = SpawnTopDown(brick, brickPosition.position);
                        break;
                    case SpawnAnimation.LeftRight:
                        brickTween = SpawnLeftRight(brick, brickPosition.position);
                        break;
                    case SpawnAnimation.Scale:
                        brickTween = SpawnScale(brick);
                        break;
                }

                sequence.Insert(0, brickTween);
                brick.OnBrickDestroyed += levelManager.OnBrickDestroyed;
                totalBricks = brick.brickType == Brick.BrickType.Destructible ? totalBricks + 1 : totalBricks;
            }
            sequence.OnComplete(() => callBack?.Invoke());
            return totalBricks;
        }

        private Tween SpawnTopDown(Brick brick, Vector3 endPosition)
        {
            var startPosition = Utils.GetRandomPointOutisdeCamera(Camera.main, new Vector2(0, 1f), new Vector2(1.5f, 2f));
            return brick.SpawnPosition(startPosition, endPosition, new Vector2(1f, 2f));
        }

        private Tween SpawnLeftRight(Brick brick, Vector3 endPosition)
        {
            var startPosition = brick.transform.position.x > LevelWidth / 2 ?
                    Utils.GetRandomPointOutisdeCamera(Camera.main, new Vector2(1.5f, 2f), new Vector2(0.5f, 1f)) :
                    Utils.GetRandomPointOutisdeCamera(Camera.main, new Vector2(-1f, -1.5f), new Vector2(0.5f, 1f));

            return brick.SpawnPosition(startPosition, endPosition, new Vector2(0.5f, 1.5f));
        }

        private Tween SpawnScale(Brick brick)
        {
            return brick.SpawnScale(brick.transform.localScale);
        }

        #endregion

        [MenuItem("Arkanoid/ New Level")]
        public static void CreateLevel()
        {
            var newLevel = ScriptableObject.CreateInstance<LevelData>();
            // TODO GET LAST ScriptableObject NAME        
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/new Level.asset");

            AssetDatabase.CreateAsset(newLevel, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}