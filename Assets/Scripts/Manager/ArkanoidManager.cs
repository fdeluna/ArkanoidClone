using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Level;
using PowerUps;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Manager
{

    public enum GameState { MENU, LOADING, PLAYING, GAMEOVER}

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ArkanoidManager : MonoBehaviour
    {
        #region Game Configuration

        [Header("Game Configuration")]
        public int MaxPowerUps = 2;
        public float Countdown = 60;
        [Space]

        [HideInInspector]
        public LevelData levelData;
        static int _totalBricks;

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
            set
            {
                _background = value;
            }
        }
        private Transform _background;

        private GameObject _walls;
        
        #endregion

        #region References
        
        public int TotalBalls { get; set; } = 1;
        public bool FreezeTime { get; set; } = false;

        private PaddleController _paddle;
        private BallController _ball;
        private UIController _ui;
        private bool _loadingLevel = false;

        private string _endGame = "GAME OVER";
        private float _currentTime;
        
        #endregion

        #region Sigleton

        public static ArkanoidManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<ArkanoidManager>();

                if (_instance != null) return _instance;

                var goName = typeof(ArkanoidManager).ToString();
                var go = GameObject.Find(goName);

                if (go != null) return _instance;

                go = new GameObject { name = goName };
                _instance = go.AddComponent<ArkanoidManager>();
                return _instance;
            }
        }
        private static ArkanoidManager _instance;

        #endregion

        #region States
        
        private GameState _currentState = GameState.MENU;
        public GameState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                switch (_currentState)
                {
                    case GameState.MENU:
                        _walls.SetActive(false);
                        _ui.StartScreen.gameObject.SetActive(true);
                        _ui.GameScreen.gameObject.SetActive(false);

                        // TODO UPDATE FLAGS IN METHOD RESET STATES
                        _currentTime = Countdown;
                        TotalBalls = 0;
                        StartCoroutine(StartGame());

                        break;
                    case GameState.LOADING:
                        _walls.SetActive(true);
                        _ui.StartScreen.gameObject.SetActive(false);
                        _ui.GameScreen.gameObject.SetActive(true);

                        StartCoroutine(Loading());
                        
                        break;
                    case GameState.PLAYING:
                        StartCoroutine(Playing());
                        break;
                    case GameState.GAMEOVER:
                        _ui.LoseScreen.gameObject.SetActive(true);
                        StartCoroutine(Lose());
                        break;
                }
            }
        }

        private IEnumerator StartGame()
        {
            yield return _ui.StartUp();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            _ui.StartGame();
            
            yield return new WaitForSeconds(1);
            
            CurrentState = GameState.LOADING;
        }
        
        private IEnumerator Playing()
        {
            while (_currentTime > 0 && CurrentState == GameState.PLAYING)
            {
                if (!_loadingLevel) 
                {
                    _currentTime -= FreezeTime ? 0 : Time.deltaTime;
                    _ui.UpdateTime(_currentTime);
                }
                yield return null;
            }

            // TODO GAME OVER EFFECTS
            // TODO UPDATE FLAGS
            CurrentState = GameState.GAMEOVER;
        }


        private IEnumerator Loading()
        {
            yield return StartCoroutine(LoadLevel());
            _paddle.PaddleSpawn();            

            CurrentState = GameState.PLAYING;
        }
        
        private IEnumerator LoadLevel()
        {
            _paddle.move = false;
            _ball?.Destroy();
            yield return new WaitForSeconds(0.25f);
            CleanLevel();
            _loadingLevel = true;            
            PowerUpManager.Instance.Init(levelData);            
            _totalBricks = levelData.Load(this, () => _loadingLevel = false);
            _ui.LodalLevelName(levelData.levelName, 1.5f);
            yield return new WaitUntil(() => !_loadingLevel);
            _ui.RemoveLevelName();
            _ball = _paddle.BallSpawn();
            _paddle.move = true;
        }

        private IEnumerator Lose()
        {
            _ball.Destroy();
            _paddle.PaddleDead();
            yield return _ui.GameOverLost(_endGame);
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Utils

        private void CleanLevel()
        {
            foreach (Transform t in Bricks)
            {
                var brick = t.GetComponent<Brick>();
                brick.OnBrickDestroyed -= OnBrickDestroyed;
                Destroy(brick.gameObject);
            }
        }

        private void LoadNextLevel()
        {
            levelData = levelData.nextLevel;
            if (levelData != null)
            {
                StartCoroutine(LoadLevel());
            }
            else
            {
                _endGame = "YOU WIN !!!";
                 CurrentState = GameState.GAMEOVER;
            }
        }        

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _walls = transform.Find("Walls").gameObject;
            _ui = FindObjectOfType<UIController>();
            _paddle = FindObjectOfType<PaddleController>();            
            CleanLevel();
        }

        private void Start()
        {
            CurrentState = GameState.MENU;
        }

#if UNITY_EDITOR
        private void Update()
        {            
            if (Application.isEditor && Application.isPlaying)
            {
                if (Input.GetKeyUp(KeyCode.S))
                {
                    StartCoroutine(LoadLevel());
                }

                if (Input.GetKeyUp(KeyCode.N))
                {
                    LoadNextLevel();
                }
            }
        }
#endif

        #endregion

        #region Events Delegates

        public void OnBrickDestroyed(Brick brick)
        {
            _totalBricks--;
            PowerUpManager.Instance.SpawnPowerUp(brick.transform.position);
            _currentTime += 1;
            if (_totalBricks > 0) return;
            
            LoadNextLevel();
        }

        public void OnBallDestroyed(float seconds)
        {
            _currentTime -= seconds;
            //TotalBalls--;
            if (TotalBalls > 0) return;                        
        }

        #endregion
    }
}