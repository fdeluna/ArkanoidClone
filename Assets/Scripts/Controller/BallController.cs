using System;
using DG.Tweening;
using Level;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    public class BallController : MonoBehaviour
    {
        #region movement

        [Range(5, 10)]
        [SerializeField]
        public float speed;
        [SerializeField]
        public float deviation = 0.3f;

        public GameObject BrickCollision;
        public GameObject PaddleCollision;

        #endregion

        #region Flags

        [HideInInspector]
        public bool magnet = false;
        [HideInInspector]
        public bool superBall = false;
        [HideInInspector]
        public bool explosiveBall = false;
        [HideInInspector]
        public bool isLaunched;
        [HideInInspector]
        public bool powerUpBall;
        private bool _move;
        private Vector3 _initScale;

        #endregion

        #region Collisions

        private Vector3 _direction = new Vector2(0.15f, 1f);
        private float _contactPointX;
        private PaddleController _paddle;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider2D;
        private Collider2D _lastCollision;
        private bool _brickHit;
        private int _layerMask;

        #endregion

        #region Effects

        private Transform _sprite;
        private TrailRenderer _trail;

        #endregion

        #region events

        private ArkanoidManager _arkanoidManager;

        public delegate void BallDestroyed(float seconds = 0);

        public event BallDestroyed OnBallDestroyed;

        #endregion


        private void Awake()
        {
            _initScale = transform.localScale;
            _arkanoidManager = FindObjectOfType<ArkanoidManager>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _paddle = FindObjectOfType<PaddleController>();
            _trail = GetComponentInChildren<TrailRenderer>();
            _layerMask = (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Walls")) |
                         (1 << LayerMask.NameToLayer("Player"));
            _sprite = transform.Find("Sprite");
        }

        private void OnEnable()
        {
            _collider2D.enabled = false;
            OnBallDestroyed -= _arkanoidManager.OnBallDestroyed;
            OnBallDestroyed += _arkanoidManager.OnBallDestroyed;            
            Reset();
            transform.DOScale(_initScale, 0.25f).SetEase(Ease.InOutElastic).SetDelay(0.25f).OnComplete(() =>
            {
                powerUpBall = false;
                _trail.enabled = true;
                _move = true;
            });
        }

        private void OnDisable()
        {
            OnBallDestroyed -= _arkanoidManager.OnBallDestroyed;
        }

        public void Update()
        {
            if (!_move) return;
            if (!isLaunched)
            {
                var paddlePosition = _paddle.transform.position;
                var position = !magnet ? new Vector3(paddlePosition.x, paddlePosition.y + 0.5f) : new Vector3(paddlePosition.x - _contactPointX, paddlePosition.y + 0.5f);
                _rigidBody.position = position;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isLaunched = true;
                    _collider2D.enabled = true;
                }
            }
        }

        private void LateUpdate()
        {
            _brickHit = false;
        }

        private void FixedUpdate()
        {
            if (isLaunched)
            {
                _rigidBody.MovePosition(_rigidBody.position +
                                    _direction.ToVector2().normalized * (speed * Time.fixedDeltaTime));
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_lastCollision != collision.collider)
            {
                _lastCollision = collision.collider;
                //_rigidBody.position += (_direction.ToVector2() * collision.collider.Distance(_collider2D).distance);

                var newDirection = Vector2.Reflect(_direction, collision.GetContact(0).normal).normalized;
                speed = Mathf.Clamp(speed + 0.1f, 4, 10);

                switch (collision.gameObject.tag)
                {
                    case "Player":
                        if (_rigidBody.position.y < _paddle.transform.position.y) return;

                        RaycastHit2D hit = Physics2D.Raycast(_rigidBody.position, _direction);

                        if (magnet)
                        {
                            isLaunched = false;
                            _contactPointX = _paddle.transform.position.x - hit.point.x;
                        }
                        else
                        {
                            var center = collision.collider.bounds.center;
                            newDirection.x += center.x > hit.point.x ? -deviation : deviation;
                            _paddle.PaddlePunch();
                            CollisionEffect(false);
                        }
                        break;
                    case "Brick":
                        if (!_brickHit)
                        {
                            _brickHit = true;
                            var brick = collision.gameObject.GetComponent<Brick>();
                            brick.Hit();

                            newDirection = new Vector2
                            {
                                x = newDirection.x + (newDirection.x > 0 ? 0.1f : -0.1f),
                                y = newDirection.y + (newDirection.y > 0 ? 0.1f : -0.1f)
                            };

                            if (superBall)
                            {
                                _brickHit = false;
                                newDirection = _direction;
                            }
                            else
                            {
                                float radius = explosiveBall ? 1 : 10;
                                var bricks = Physics2D.OverlapCircleAll(brick.transform.position, radius, 1 << LayerMask.NameToLayer("Brick"));

                                for (int i = 0; i < bricks.Length; i++)
                                {
                                    var neighborBrick = bricks[i].GetComponent<Brick>();
                                    float delay = Vector2.Distance(neighborBrick.transform.position, brick.transform.position) / 10;
                                    Vector3 direction = (neighborBrick.transform.position - brick.transform.position).normalized;

                                    if (explosiveBall)
                                    {
                                        neighborBrick.Hit();
                                    }
                                    else
                                    {
                                        neighborBrick?.HitAnimation(delay, direction);
                                    }
                                }
                            }
                        }
                        break;
                    case "DeadZone":
                        if (powerUpBall)
                        {
                            Destroy();
                        }
                        else
                        {
                            OnBallDestroyed.Invoke(5);
                            speed = Mathf.Clamp(speed / 2f, 5, 10);
                        }
                        break;
                }

                CollisionEffect();
                _direction = newDirection;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _brickHit = false;
        }

        private void CollisionEffect(bool brick = true)
        {
            var angle = Mathf.Atan2(_direction.x, _direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            PoollingPrefabManager.Instance.GetPooledPrefab(brick ? BrickCollision : PaddleCollision, transform.position);
        }

        public void Destroy()
        {
            CollisionEffect();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            speed = 5;
            isLaunched = false;
            _direction = new Vector2(0.15f, 1f);
            ResetPowerUps();
            transform.localScale = Vector3.zero;
        }

        #region Power Ups

        public void ResetPowerUps()
        {
            magnet = false;
            superBall = false;
        }

        public BallController InstantiatePowerUpBall(Vector3 position)
        {
            var ball = PoollingPrefabManager.Instance.GetPooledPrefab(gameObject, position)
                .GetComponent<BallController>();
            ball.isLaunched = true;
            ball.powerUpBall = true;
            ball._direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            return ball;
        }


        #endregion
    }
}