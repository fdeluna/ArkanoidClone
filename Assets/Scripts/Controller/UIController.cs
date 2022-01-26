using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;

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


        public Transform LoseScreen;
        private Image _gameOverBackground;
        private TextMeshProUGUI _gameOverText;


        private void Awake()
        {
            _fade = transform.Find("Fade").GetComponent<Image>();
            StartScreen = transform.Find("Start Screen");
            _mainTitleText = StartScreen.Find("Title").GetComponent<TextMeshProUGUI>();
            _pressKeyText = StartScreen.Find("Press Key").GetComponent<TextMeshProUGUI>();

            GameScreen = transform.Find("Game Screen");
            _timeText = GameScreen.Find("Time").GetComponent<TextMeshProUGUI>();
            _levelTitle = GameScreen.Find("Title").GetComponent<TextMeshProUGUI>();


            LoseScreen = transform.Find("Lose Screen");
            _gameOverBackground = LoseScreen.Find("Background").GetComponent<Image>();
            _gameOverText = LoseScreen.Find("Text").GetComponent<TextMeshProUGUI>();

            _levelTitle.text = "";
            _timeText.text = "";
        }

        #region Start Game

        public IEnumerator StartUp()
        {
            _pressKeyText.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
            yield return StartCoroutine(_fade.IFadeInOut(false, 2, Color.black));
        }

        public void StartGame()
        {
            _mainTitleText.rectTransform.DOAnchorPos(new Vector2(-800, 0), 1).SetEase(Ease.InBack);
            _pressKeyText.rectTransform.DOAnchorPos(new Vector2(800, 0), 1).SetEase(Ease.InBack);
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

        #region Timer

        public void UpdateTime(float currentTime)
        {
            int minutes =  (int)(currentTime / 60f);
            int seconds = (int)(currentTime % 60f);

            _timeText.text = string.Format("{0:00}:{1:00}", minutes,seconds);
        }


        // TODO FOR EFFECTS
        //public void DamageTime(float currentTime, float damage)
        //{

        //}

        //public void AddTime(float currentTime, float damage)
        //{

        //}

        //public void FreezeTime(float currentTime, float damage)
        //{

        //}
        #endregion

        #region GameOver

        public IEnumerator GameOverLost(string text)
        {
            Sequence sequence = DOTween.Sequence();
            _gameOverText.text = text;
            sequence.Append(_gameOverBackground.DOFade(1f, 2f).SetEase(Ease.InOutSine));
            sequence.Append(_gameOverText.DOFade(1, 3f).SetEase(Ease.InOutSine));

            yield return new WaitUntil(() => !sequence.IsActive());
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