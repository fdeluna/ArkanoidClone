using DG.Tweening;
using UnityEngine;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource sfx;
        [SerializeField]
        private AudioSource background;
        
        [Header("Music")]
        [SerializeField]
        private AudioClip startGame;
        [SerializeField]
        private AudioClip gameBackground;
        [SerializeField]
        private AudioClip startLevel;
        [SerializeField]
        private AudioClip winLevel;
        [SerializeField]
        private AudioClip gameOver;
        
        [Header("SFX")]
        [SerializeField]
        private AudioClip pickPowerUp;
        [SerializeField]
        private AudioClip activePowerUp;
        [SerializeField]
        private AudioClip loseTime;
        [SerializeField]
        private AudioClip brickCollision;
        [SerializeField]
        private AudioClip ballCollision;

        
        public void StartGame() => PlayOneShot(startGame);
        public void GameBackground() => PlayBackground(gameBackground);
        public void StartLevel() => PlayOneShot(startLevel);
        public void WinLevel() => PlayBackground(winLevel);
        public void GameOverLevel() => PlayBackground(gameOver);
        
        public void PickPowerUp() => PlayOneShot(pickPowerUp);
        public void ActivePowerUp() => PlayOneShot(activePowerUp);
        public void LoseTime() => PlayOneShot(loseTime);
        public void BrickCollision() => PlayOneShot(brickCollision);
        public void BallCollision()
        {
            DOVirtual.DelayedCall(0.1f, () => PlayOneShot(ballCollision));
        }


        private void PlayBackground(AudioClip clip)
        {
            background.loop = true;
            background.Stop();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                background.clip = clip;
                background.Play();
            });
        }
        
        private void PlayOneShot(AudioClip clip)
        {
            sfx.PlayOneShot(clip);
        }
        
        #region Sigleton

        public static AudioManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<AudioManager>();

                if (_instance != null) return _instance;

                var goName = typeof(ArkanoidManager).ToString();
                var go = GameObject.Find(goName);

                if (go != null) return _instance;

                go = new GameObject { name = goName };
                _instance = go.AddComponent<AudioManager>();
                return _instance;
            }
        }

        private static AudioManager _instance;

        #endregion

    }
}

