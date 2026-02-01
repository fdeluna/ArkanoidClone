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

        [Range(4, 8f)] [SerializeField] public float speed;
        [SerializeField] public float deviation = 0.5f;

        public LayerMask layerMask;

        public GameObject BrickCollision;
        public GameObject PaddleCollision;

        public Color powerUpBallColor;
        public Gradient trailPowerUpBallColor;

        #endregion

        #region Flags

        public bool magnet = false;
        public bool superBall = false;
        public bool explosiveBall = false;
        public bool isLaunched;
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
        private int _layerMask;

        #endregion

        #region Effects

        private SpriteRenderer _sprite;
        private TrailRenderer _trail;


        private Color initBallColor;
        private Gradient initTrailcolor;
        private Vector3 _lastFramePosition;

        #endregion

        #region events

        private ArkanoidManager _arkanoidManager;

        public delegate void BallDestroyed(float seconds = 0);

        public event BallDestroyed OnBallDestroyed;
        
        public delegate void PaddleHit(float seconds = 0);

        public event PaddleHit OnPaddleHit;

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
            _sprite = GetComponentInChildren<SpriteRenderer>();

            initBallColor = _sprite.color;
            initTrailcolor = _trail.colorGradient;
        }

        private void OnEnable()
        {
            _sprite.color = initBallColor;
            _trail.colorGradient = initTrailcolor;
            _collider2D.enabled = false;
            powerUpBall = false;
            _trail.enabled = false;

            Reset();

            transform.DOScale(_initScale, 0.25f).SetEase(Ease.InOutElastic).SetDelay(0.25f).OnComplete(() =>
            {
                _trail.enabled = true;
                _move = true;
            });

            OnBallDestroyed -= _arkanoidManager.OnBallDestroyed;
            OnBallDestroyed += _arkanoidManager.OnBallDestroyed;

            OnPaddleHit -= _arkanoidManager.AddTime;
            OnPaddleHit += _arkanoidManager.AddTime;
        }

        private void OnDisable()
        {
            OnBallDestroyed -= _arkanoidManager.OnBallDestroyed;
            OnPaddleHit -= _arkanoidManager.AddTime;
        }

        public void Update()
        {
            if (!_move) return;
            if (!isLaunched)
            {
                var paddlePosition = _paddle.transform.position;
                var position = !magnet
                    ? new Vector3(paddlePosition.x, paddlePosition.y + 0.5f)
                    : new Vector3(paddlePosition.x - _contactPointX, paddlePosition.y + 0.5f);
                _rigidBody.position = position;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isLaunched = true;
                    _collider2D.enabled = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (isLaunched)
            {
                Collider2D collider =
                    Physics2D.OverlapBox(_rigidBody.position, transform.localScale.ToVector2() * 1.1f, 0, layerMask);
                
                if (collider != null)
                {
                    HandleCollision(collider);
                }

                _rigidBody.MovePosition(_rigidBody.position +
                                        _direction.ToVector2().normalized * (speed * Time.fixedDeltaTime));
            }
        }

        private void LateUpdate()
        {
            _lastFramePosition = _rigidBody.position;
        }

        private void HandleCollision(Collider2D collider)
        {
            if (_lastCollision != collider)
            {
                RaycastHit2D hit = Physics2D.BoxCast(_rigidBody.position, transform.localScale.ToVector2() * 1.1f, 0,
                    _direction, Mathf.Infinity, layerMask);
                
                var newDirection = Vector2.Reflect(_direction, hit.normal).normalized;

                speed = Mathf.Clamp(speed + 0.2f, 4, 8);
                
                AudioManager.Instance.BallCollision();

                switch (collider.tag)
                {
                    case "Player":

                        if (_direction.y > 0)
                        {
                            return;
                        }

                        if (magnet)
                        {
                            isLaunched = false;
                            _contactPointX = _paddle.transform.position.x - hit.point.x;
                            newDirection = new Vector2(0.1f, 1f);
                        }
                        else
                        {
                            var center = collider.bounds.center;
                            newDirection.x += center.x > hit.point.x ? -deviation : deviation;
                        }

                        OnPaddleHit?.Invoke(1f);
                        _paddle.PaddlePunch();

                        break;
                    case "Brick":
                        var brick = collider.gameObject.GetComponent<Brick>();
                        AudioManager.Instance.BrickCollision();
                        brick.Hit(powerUpBall || superBall);

                        newDirection = new Vector2
                        {
                            x = newDirection.x + (newDirection.x > 0 ? 0.1f : -0.1f),
                            y = newDirection.y + (newDirection.y > 0 ? 0.1f : -0.1f)
                        };
                        if (superBall)
                        {
                            newDirection = _direction;
                        }
                        else
                        {
                            float radius = explosiveBall ? 1.5f : 10;
                            var bricks = Physics2D.OverlapCircleAll(brick.transform.position, radius, 1 << LayerMask.NameToLayer("Brick"));

                            for (int i = 0; i < bricks.Length; i++)
                            {
                                var neighborBrick = bricks[i].GetComponent<Brick>();
                                float delay = Vector2.Distance(neighborBrick.transform.position, brick.transform.position) / 30f;
                                Vector3 direction = (neighborBrick.transform.position - brick.transform.position)
                                    .normalized * 0.2f;

                                if (explosiveBall)
                                {
                                    neighborBrick.Hit(explosiveBall);
                                }
                                else
                                {
                                    neighborBrick?.HitAnimation(delay, direction);
                                }
                            }
                        }

                        break;
                    case "DeadZone":

                        if (!powerUpBall)
                        {
                            OnBallDestroyed.Invoke(1f);
                            speed = Mathf.Clamp(speed * 0.5f, 4, 8);
                            Camera.main.transform.DOShakePosition(0.5f, 0.5f);
                        }

                        // THIS IS A WALL
                        Transform sprite = collider.transform.Find("Sprite");
                        sprite.DOPunchPosition(Vector3.right * 2f, 0.3f).SetEase(Ease.InOutExpo)
                            .OnComplete(() => sprite.localPosition = Vector3.zero);

                        break;
                    default:
                        // THIS IS A WALL
                        sprite = collider.transform.Find("Sprite");
                        sprite.DOPunchPosition(Vector3.right * 2f, 0.2f).SetEase(Ease.InOutExpo)
                            .OnComplete(() => sprite.localPosition = Vector3.zero);
                        break;
                }

                _lastCollision = collider;
                CollisionEffect();
                _rigidBody.position = _lastFramePosition;
                _direction = newDirection;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("SafeZone"))
            {
                _direction = new Vector2(0.15f, 1f);
                var paddlePosition = _paddle.transform.position;
                _rigidBody.position = new Vector3(paddlePosition.x, paddlePosition.y + 0.5f);
            }
        }

        private void CollisionEffect()
        {
            float angle = Mathf.Atan2(_direction.x, _direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            PoollingPrefabManager.Instance.GetPooledPrefab(BrickCollision, transform.position);
            PoollingPrefabManager.Instance.GetPooledPrefab(PaddleCollision, transform.position);
        }

        public void Destroy()
        {
            CollisionEffect();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            speed = 4;
            isLaunched = false;
            _direction = new Vector2(0.15f, 1f);
            transform.localScale = Vector3.zero;
            ResetPowerUps();
        }

        #region Power Ups

        public void ResetPowerUps()
        {
            magnet = false;
            superBall = false;
        }

        public BallController InstantiatePowerUpBall(Vector3 position)
        {
            var ball = PoollingPrefabManager.Instance.GetPooledPrefab(gameObject, transform.position)
                .GetComponent<BallController>();
            ball.isLaunched = true;
            ball.powerUpBall = true;
            ball.speed = 8;
            ball._sprite.color = powerUpBallColor;
            ball._trail.colorGradient = trailPowerUpBallColor;
            ball.transform.localScale = _initScale;
            ball._direction = new Vector3(Random.Range(-1f, 1f), 1);
            return ball;
        }

        #endregion
    }
}