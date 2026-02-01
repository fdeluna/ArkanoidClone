
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Controller
{
    [ExecuteInEditMode]
    public class UIController : MonoBehaviour
    {
        public Transform StartScreen;
        private TextMeshProUGUI _mainTitleText;
        private TextMeshProUGUI _pressKeyText;


        public Transform GameScreen;
        private TextMeshProUGUI _levelTitle;
        private TextMeshProUGUI _timeText;
        private Image _fade;

        public Transform tutorialScreen;
        public RectTransform rulesScreen;
        public RectTransform powerUpScreen;
        public TextMeshProUGUI pressSpaceKey;


        public Transform LoseScreen;
        private TextMeshProUGUI _gameOverText;


        private Color _timeStartColor;
        private Tween _timeColorTween;
        private bool _freezeTime = false;


        private void Awake()
        {
            _fade = transform.Find("Fade").GetComponent<Image>();
            StartScreen = transform.Find("Start Screen");
            _mainTitleText = StartScreen.Find("Title").GetComponent<TextMeshProUGUI>();

            GameScreen = transform.Find("Game Screen");
            _timeText = GameScreen.Find("Time").GetComponent<TextMeshProUGUI>();
            _levelTitle = GameScreen.Find("Title").GetComponent<TextMeshProUGUI>();


            LoseScreen = transform.Find("Lose Screen");
            _gameOverText = LoseScreen.Find("Text").GetComponent<TextMeshProUGUI>();

            _levelTitle.text = "";
            _timeText.text = "";

            _timeStartColor = _timeText.color;
        }

        #region Start Game

        public IEnumerator StartUp()
        {
            if (FeelConfiguration.Instance.settings.startUp)
            {
                pressSpaceKey.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
                yield return StartCoroutine(_fade.IFadeInOut(false,3, Color.black));   
            }
        }
        
        public IEnumerator GameOver()
        {
            yield return StartCoroutine(_fade.IFadeInOut(true, 3, Color.black));
        }
        
        public void StartTutorial()
        {
            _mainTitleText.rectTransform.DOAnchorPos(new Vector2(-800, 0), 1).SetEase(Ease.InBack);
        }

        public void TutorialRules()
        {
            tutorialScreen.gameObject.SetActive(true);
            rulesScreen.gameObject.SetActive(true);
            powerUpScreen.gameObject.SetActive(false);

            rulesScreen.anchoredPosition = new Vector2(-800, 0);
            rulesScreen.DOAnchorPos(new Vector2(0, 0), 1).SetEase(Ease.InBack);
        }
        
        public void TutorialPowerUps()
        {
            rulesScreen.DOAnchorPos(new Vector2(800, 0), 1).SetEase(Ease.InBack).OnComplete(() =>
            {
                rulesScreen.gameObject.SetActive(false);
                powerUpScreen.gameObject.SetActive(true);

                powerUpScreen.anchoredPosition = new Vector2(-800, 0);
                powerUpScreen.DOAnchorPos(new Vector2(0, 0), 1).SetEase(Ease.InBack);
            });
        }

        public void StartGame()
        {
            powerUpScreen.DOAnchorPos(new Vector2(800, 0), 1).SetEase(Ease.InBack)
                .OnComplete(() => tutorialScreen.gameObject.SetActive(false));
            pressSpaceKey.rectTransform.DOAnchorPos(new Vector2(800, 0), 1).SetEase(Ease.InBack);
        }

        #endregion

        #region Loading Level

        public void LodalLevelName(string levelName, float time)
        {
            int randomEffect = Random.Range(0, 3);
            _levelTitle.transform.localScale = Vector3.one;
            _levelTitle.text = levelName;

            switch (randomEffect)
            {
                case 0:
                    _levelTitle.FadeText(time);
                    break;
                case 1:
                    _levelTitle.SinWaveFadeMove(40, time / 2);
                    break;
                case 2:
                    _levelTitle.RandomFadeRotation(3, time);
                    break;
            }
        }

        public void RemoveLevelName()
        {
            _levelTitle.transform.DOScale(0, 1f).SetEase(Ease.InOutQuart);
        }

        public void SetLevelName(string levelName)
        {
            _levelTitle.text = levelName;
        }

        #endregion


        public bool damage;

        private void Update()
        {
            if (damage)
            {
                AddTime();
                damage = false;
            }
        }

        #region Timer

        public void UpdateTime(float currentTime)
        {
            int minutes = (int)(currentTime / 60f);
            int seconds = (int)(currentTime % 60f);

            _timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }


        // TODO FOR EFFECTS
        public void DamageTime()
        {
            if (!_freezeTime)
            {
                _timeColorTween?.Kill();
                _timeColorTween = _timeText.DOColor(Color.red, 0.15f).SetEase(Ease.InOutCubic).SetAutoKill(false)
                    .OnComplete(() => _timeText.DOColor(_timeStartColor, 0.15f));
                _timeText.rectTransform.DOShakePosition(0.2f, 15, 50);
            }
        }

        public void AddTime()
        {
            if (!_freezeTime)
            {
                _timeColorTween?.Kill();
                _timeColorTween = _timeText.DOColor(Color.green, 0.15f).SetEase(Ease.InOutCubic).SetAutoKill(false)
                    .OnComplete(() => _timeText.DOColor(_timeStartColor, 0.15f));
                _timeText.rectTransform.DOScale(Vector3.one * 1.2f, 0.15f)
                    .OnComplete(() => _timeText.rectTransform.DOScale(Vector3.one, 0.1f));
            }
        }

        public void FreezeTime(bool freeze)
        {
            _freezeTime = freeze;
            _timeColorTween?.Kill();

            if (_freezeTime)
            {
                _timeText.DOColor(Color.cyan, 0.15f).SetEase(Ease.InOutCubic);
            }
            else
            {
                _timeText.DOColor(_timeStartColor, 0.15f).SetEase(Ease.InOutCubic);
            }
        }

        #endregion

        #region GameOver

        public IEnumerator GameOverLost(string text)
        {
            GameScreen.gameObject.SetActive(false);
            
            yield return new WaitForSeconds(1);
            
            LoseScreen.gameObject.SetActive(true);
            _gameOverText.text = text;

            powerUpScreen.anchoredPosition = new Vector2(-800, 0);
            powerUpScreen.DOAnchorPos(new Vector2(0, 0), 1).SetEase(Ease.InBack);
            
            pressSpaceKey.rectTransform.anchoredPosition = new Vector2(-800, 0);
            pressSpaceKey.rectTransform.DOAnchorPos(new Vector2(0, 0), 1).SetEase(Ease.InBack);
            
            yield return new WaitForSeconds(1);
        }

        #endregion

        #region Fade

        public IEnumerator FadeIn(float seconds = 1)
        {
            yield return StartCoroutine(_fade.IFadeInOut(true, seconds, Color.black));
        }

        public IEnumerator FadeOut(float seconds = 1)
        {
            yield return StartCoroutine(_fade.IFadeInOut(false, seconds, Color.black));
        }

        #endregion
    }
}