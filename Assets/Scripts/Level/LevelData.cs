using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelData : ScriptableObject
{
    enum SpawnAnimation { TopDown, LeftRight, Scale };

    public static readonly int LevelHeight = 14;
    public static readonly int LevelWidth = 16;
    public static readonly float BrickHeight = 0.5f;
    public static readonly float BrickWidth = 1;

    public Sprite BackgroundSprite;
    public AudioClip BackgroundMusic;
    public LevelData NextLevel;
    public List<BrickPosition> LevelBricks = new List<BrickPosition>();

    [Range(0, 1)]
    public float PowerUpChance = 0.1f;
    public List<PowerUpProbability> PowerUpsProbability = new List<PowerUpProbability>();

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

    public int Load(ArkanoidManager levelManager, System.Action callBack = null)
    {
        int totalBricks = 0;
        SpawnAnimation spawnAnimation = (SpawnAnimation)Random.Range(0, 3);
        Sequence sequence = DOTween.Sequence();
        foreach (BrickPosition brickPosition in LevelBricks)
        {
            Brick brick = (Instantiate(Resources.Load("Bricks/" + brickPosition.PrefabName)) as GameObject).GetComponent<Brick>();
            brick.transform.parent = levelManager.Bricks;
            brick.transform.position = brickPosition.Position;
            Tween brickTween = null;
            switch (spawnAnimation)
            {
                case SpawnAnimation.TopDown:
                    brickTween = SpawnTopDown(brick.transform, brickPosition.Position);
                    break;
                case SpawnAnimation.LeftRight:
                    brickTween = SpawnLeftRight(brick.transform, brickPosition.Position);
                    break;
                case SpawnAnimation.Scale:
                    brickTween = SpawnSacle(brick.transform);
                    break;
            }            
            sequence.Insert(0,brickTween);
            brick.OnBrickDestroyed += levelManager.OnBrickDestroyed;
            totalBricks = brick.bricktype == Brick.BrickType.DESTRUCTIBLE ? totalBricks + 1 : totalBricks;
        }
        sequence.OnComplete(() => callBack?.Invoke());
        return totalBricks;
    }


    private Tween SpawnTopDown(Transform brick, Vector3 endPosition)
    {
        Vector3 startPosition = new Vector3(brick.transform.localPosition.x, 1);
        brick.transform.localPosition = startPosition;
        return brick.transform.DOMove(endPosition, 2f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutExpo).OnComplete(() => brick.transform.position = endPosition);
    }

    private Tween SpawnLeftRight(Transform brick, Vector3 endPosition)
    {
        float x = brick.position.x > 0 ? 10 : -10;
        Vector3 startPosition = new Vector3(x, brick.transform.position.y);
        brick.transform.position = startPosition;
        return brick.transform.DOMove(endPosition, 1.5f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutExpo).OnComplete(() => brick.transform.position = endPosition);
    }

    private Tween SpawnSacle(Transform brick)
    {
        Vector3 endScale = brick.localScale;
        brick.transform.localScale = Vector3.zero;
        return brick.transform.DOScale(endScale, 1.5f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutElastic).OnComplete(() => brick.transform.localScale = endScale);
    }


    public void Clean(ArkanoidManager levelManager)
    {
        foreach (Transform t in levelManager.Bricks)
        {
            Brick brick = t.GetComponent<Brick>();
            brick.OnBrickDestroyed -= levelManager.OnBrickDestroyed;
            Destroy(brick.gameObject);
        }
    }

    [MenuItem("Arkanoid/ New Level")]
    public static void CreateLevel()
    {
        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
        // TODO GET LAST SCRIPTABLEOBJECT NAME        
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/new Level.asset");
        AssetDatabase.CreateAsset(newLevel, assetPathAndName);
        List<GameObject> powerUps = Utils.GetPrefabsAtPath(Utils.POWERUPS_PATH);

        foreach (GameObject powerUpPrefab in powerUps)
        {
            newLevel.PowerUpsProbability.Add(new PowerUpProbability() { powerUp = powerUpPrefab });
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}