using UnityEngine;

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
    public Transform _bricks;

    // AQUI FALTA MAPEAR LAS VARIABLES DEL SCRIPTABLEoBJECT
}
