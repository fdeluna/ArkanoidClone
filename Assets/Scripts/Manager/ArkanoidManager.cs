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
    public enum GameState
    {
        MENU,
        LOADING,
        PLAYING,
        GAMEOVER
    }

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ArkanoidManager : MonoBehaviour
    {
        #region Game Configuration

        [Header("Game Configuration")] public int MaxPowerUps = 2;
        public float Countdown = 60;
        [Space] [HideInInspector] public LevelData levelData;
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
            set { _background = value; }
        }

        private Transform _background;
        private bool _win = false;

        private GameObject _fireWorks;
        private GameObject _walls;

        #endregion

        #region References

        public int TotalBalls { get; set; } = 1;
        public BallController ball;

        private PaddleController _paddle;
        private UIController _ui;
        public bool loadingLevel = false;

        private string _endGame = "GAME OVER";
        private float _currentTime;
        private bool _freezeTime = false;

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
            get { return _currentState; }
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
                        StartCoroutine(GameOver());
                        break;
                }
            }
        }

        private IEnumerator StartGame()
        {
            yield return _ui.StartUp();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            
            AudioManager.Instance.StartGame();
            _ui.StartTutorial();
            yield return new WaitForSeconds(1);

            _ui.TutorialRules();
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));


            _ui.TutorialPowerUps();
            yield return new WaitForSeconds(2);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            
            AudioManager.Instance.GameBackground();
            _ui.StartGame();

            yield return new WaitForSeconds(1);

            CurrentState = GameState.LOADING;
        }

        private IEnumerator Playing()
        {
            while (_currentTime > 0 && CurrentState == GameState.PLAYING)
            {
                if (!loadingLevel)
                {
                    _currentTime -= _freezeTime ? 0 : Time.deltaTime;
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
            ball?.Destroy();
            yield return new WaitForSeconds(0.25f);
            AudioManager.Instance.StartLevel();
            CleanLevel();
            loadingLevel = true;
            PowerUpManager.Instance.Init(levelData);
            _totalBricks = levelData.Load(this, () => loadingLevel = false);
            _ui.LodalLevelName(levelData.levelName, 1.5f);
            yield return new WaitUntil(() => !loadingLevel);
            _ui.RemoveLevelName();
            ball = _paddle.BallSpawn();
            _paddle.move = true;
        }

        private IEnumerator GameOver()
        {
            PowerUpManager.Instance.CleanPowerUps();
            StartCoroutine(_ui.GameOverLost(_endGame));
            ball.Destroy();
            _paddle.PaddleDead();
            
            if (_win)
            {
                _fireWorks.gameObject.SetActive(true);
                AudioManager.Instance.WinLevel();
            }
            else
            {
                _fireWorks.gameObject.SetActive(false);
                AudioManager.Instance.GameOverLevel();
            }

            foreach (Transform t in Bricks)
            {
                var brick = t.GetComponent<Brick>();
                if (brick.gameObject.activeInHierarchy)
                {
                    brick.OnBrickDestroyed -= OnBrickDestroyed;
                    brick.Hit(true);
                }

                Destroy(brick.gameObject);
            }

            _instance = null;
            yield return new  WaitForSeconds(1);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return _ui.GameOver();
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
                _win = true;
                CurrentState = GameState.GAMEOVER;
            }
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _walls = transform.Find("Walls").gameObject;
            _fireWorks = transform.Find("Fireworks").gameObject;
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
            if (_currentState == GameState.PLAYING)
            {
                _totalBricks--;

                if (brick.addTime)
                {
                    AddTime(1.2f);
                    PowerUpManager.Instance.SpawnPowerUp(brick.transform.position);
                }

                if (_totalBricks > 0) return;
                
                _currentTime += 10f;
                _ui.AddTime();
                LoadNextLevel();
            }
        }

        public void AddTime(float time)
        {
            _currentTime += time;
            _ui.AddTime();
        }

        public void OnBallDestroyed(float seconds)
        {
            _currentTime = Mathf.Clamp(_currentTime - seconds, 0, Mathf.Infinity);
            _ui.UpdateTime(_currentTime);
            _ui.DamageTime();
            AudioManager.Instance.LoseTime();
        }

        public void FreezeTime(bool freeze)
        {
            _freezeTime = freeze;
            _ui.FreezeTime(freeze);
        }

        #endregion
    }
}