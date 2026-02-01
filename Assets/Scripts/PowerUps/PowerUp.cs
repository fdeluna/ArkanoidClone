using Controller;
using Manager;
using UnityEngine;

namespace PowerUps
{
    public enum PowerUpType { GROW, SHRINK, GUN, MAGNET, MULTIBALL, RANDOMMOVE, SUPERBALL, SWAPMOVE, FREEZE, EXPLOSIVE }
    public abstract class PowerUp : MonoBehaviour
    {
        public PowerUpType type;
        public GameObject particles;
        public float fallSpeed = 5;
        public float applicationTime = 2f;

        [HideInInspector]
        public bool Active = false;
        [HideInInspector]
        public float CurrentTime = 0f;

        protected PaddleController Paddle;
        protected BallController Ball;

        private bool _falling = true;
        private Collider2D _collider;

        private void Awake()
        {
            Paddle = FindObjectOfType<PaddleController>();
            _collider = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            Active = false;
            _falling = true;
            _collider.enabled = true;
            CurrentTime = applicationTime;
            Ball = ArkanoidManager.Instance.ball;
        }

        private void Update()
        {
            if (_falling)
            {
                transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime));
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                AudioManager.Instance.PickPowerUp();
                PowerUpManager.Instance.AddPowerUp(this);
                _collider.enabled = false;
                _falling = false;
            }
            else
            {
                DestroyPowerUp();
            }
            PoollingPrefabManager.Instance.GetPooledPrefab(particles, transform.position);
        }

        public abstract void ApplyPowerUp();

        public virtual void UnApplyPowerUp()
        {
            gameObject.SetActive(false);
        }

        private void DestroyPowerUp()
        {            
            // TODO PARTICLES POWER UP
            gameObject.SetActive(false);
        }
    }
}
