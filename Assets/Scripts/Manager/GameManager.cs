using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelManager LevelManager;    
    public static int totalBricks = 0;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    string goName = typeof(GameManager).ToString();
                    GameObject go = GameObject.Find(goName);

                    if (go == null)
                    {
                        go = new GameObject();
                        go.name = goName;
                        _instance = go.AddComponent<GameManager>();
                    }

                }
            }
            return _instance;
        }
    }
    private static GameManager _instance;

    private void Awake()
    {
        LevelManager = FindObjectOfType<LevelManager>();        
    }

    private void Start()
    {
        LevelManager.LoadLevel();
    }
    



}