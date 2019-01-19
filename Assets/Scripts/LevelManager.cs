using UnityEngine;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{
    public LevelData LevelData;
    static int totalBricks;

    // TODO GAMEOBJECT
    public GameObject PowerUp;

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
    // AQUI FALTA MAPEAR LAS VARIABLES DEL SCRIPTABLEOBJECT

    public void LoadLevel()
    {
        CleanLevel();
        totalBricks = LevelData.Load(this);
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

        float range = Random.Range(0, 1f);
        Debug.Log(range);
        if (range <= LevelData.PowerUpChance)
        {
            Instantiate(PowerUp, brick.transform.position, Quaternion.identity);
        }

        if (totalBricks <= 0)
        {
            Debug.Log("WIN");
        }
    }
}
