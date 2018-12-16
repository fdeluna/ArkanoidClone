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
    // AQUI FALTA MAPEAR LAS VARIABLES DEL SCRIPTABLEOBJECT

    void Awake()
    {
        CleanLevel();
    }

    private void Start()
    {
        LevelData.Load(this);
    }

    void CleanLevel()
    {
        foreach (Transform t in Bricks)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

}
