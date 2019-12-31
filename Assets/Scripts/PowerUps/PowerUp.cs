using Controller;
using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public GameObject particles;
        public float fallSpeed = 5;

        protected PaddleController Paddle;
        protected BallController Ball;

        private Collider2D _collider;

        private void Awake()
        {
            Paddle = FindObjectOfType<PaddleController>();
            _collider = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            _collider.enabled = true;

            // Needed here due to there could be more than one ballController for MultiBallPowerUp
            foreach (var ball in FindObjectsOfType<BallController>())
            {
                if (ball.gameObject.activeInHierarchy)
                {
                    Ball = ball;
                    break;
                }
            }
        }

        private void Update()
        {
            transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Paddle.currentPowerUp = this;
            DestroyPowerUp();
        }

        protected abstract void ApplyPowerUp();


        private void DestroyPowerUp()
        {
            Paddle.ResetPowerUps();
            Ball.ResetPowerUps();
            ApplyPowerUp();
            gameObject.SetActive(false);
        }
    }
}
