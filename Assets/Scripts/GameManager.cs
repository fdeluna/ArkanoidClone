using UnityEngine;

public class GameManager : MonoBehaviour
{
    LevelManager _levelManager;
    [SerializeField]
    public static int totalBricks = 0;

    private void Awake()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start()
    {
        _levelManager.LoadLevel();
    }
}
