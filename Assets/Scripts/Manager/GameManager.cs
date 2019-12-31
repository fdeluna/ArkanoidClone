using Manager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
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
    #endregion

    public ArkanoidManager ArkanoidManager;
    public static int totalBricks = 0;

    public enum GameState { Start, LoadGame, Playing, PlayerDead, GameOver };

    public GameState CurrentState
    {
        set
        {
            _currentState = value;
            OnGameStateChanged?.Invoke(_currentState);
        }
        get
        {
            return _currentState;
        }
    }
    private GameState _currentState;

    public delegate void GameStateChanged(GameState state);
    public static event GameStateChanged OnGameStateChanged;

    private void Awake()
    {
        ArkanoidManager = FindObjectOfType<ArkanoidManager>();
    }

    private void OnEnable()
    {
        CurrentState = GameState.Start;
    }

}