using UnityEngine;

[DisallowMultipleComponent]
public class LevelInfo : MonoBehaviour
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
    private Transform _bricks;


    [HideInInspector]
    public GameObject[] LevelBricks
    {
        get
        {
            if (_levelBricks == null || _levelBricks.Length == 0)
            {
                _levelBricks = new GameObject[LevelData.LevelWidth * LevelData.LevelHeight];
            }
            return _levelBricks;
        }
    }


    private GameObject[] _levelBricks;

    // AQUI FALTA MAPEAR LAS VARIABLES DEL SCRIPTABLEoBJECT

    void Awake()
    {
        CleanLevel();        
    }

    private void Start()
    {
        LevelData.Load(this);
    }

    public void CleanLevel()
    {
        if (Bricks.childCount > 0)
        {
            foreach (Transform t in Bricks)
            {
                GameObject.DestroyImmediate(t.gameObject);
            }
        }
        _levelBricks = null;
    }
    
}
