using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StartGame : ArkanoidObject
    {
        private TextMeshProUGUI _pressStart;
        private bool _gameStarted = false;

        private void Start()
        {
            _pressStart = transform.Find("Press Key").GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _pressStart.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
            _gameStarted = false;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space) || _gameStarted) return;
            _gameStarted = true;
            _pressStart.DOFade(0, 0.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                GameManager.Instance.CurrentState = GameManager.GameState.LoadGame;
                gameObject.SetActive(false);
            });
        }


        protected override void OnGameStateChanged(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Start:
                    gameObject.SetActive(true);
                    break;
                case GameManager.GameState.LoadGame:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}
