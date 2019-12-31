using DG.Tweening;
using Level;
using Manager;
using UnityEngine;

namespace Controller
{
    public class BallController : ArkanoidObject
    {
        #region movement
        [Range(4, 8)]
        [SerializeField] 
        float speed;
        [SerializeField] 
        float deviation = 0.3f;
        #endregion

        #region Flags
        
        public bool magnet = false;
        public bool superBall = false;
        private bool _move = false;
        private bool _isLaunched = false;
        
        #endregion

        #region Collisions
        
        private Vector3 _direction = new Vector2(0.15f, 1f);
        private float _contactPointX = 0;
        private PaddleController _paddle;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider2D;
        private bool _brickHitted = false;
        private int _layerMask;
        
        #endregion

        #region Effects
        
        private Tweener _scaleTweener;
        private Transform _sprite;
        #endregion

        #region events
        public delegate void BallDestroyed();
        public event BallDestroyed OnBallDestroyed;
        #endregion

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _paddle = FindObjectOfType<PaddleController>();
            _layerMask = (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Player"));
            _sprite = transform.Find("Sprite");
        }

        private void OnEnable()
        {
            Reset();
            OnBallDestroyed -= GameManager.Instance.ArkanoidManager.OnBallDestroyed;
            OnBallDestroyed += GameManager.Instance.ArkanoidManager.OnBallDestroyed;
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutElastic).SetDelay(1.5f);
            _move = false;
        }

        private void Update()
        {
            if (!_move) return;
            
            if (!_isLaunched)
            {
                var paddlePosition = _paddle.transform.position;
                var position = !magnet ? new Vector3(paddlePosition.x, paddlePosition.y + 0.5f) : new Vector3(paddlePosition.x - _contactPointX, paddlePosition.y + 0.5f);
                _rigidBody.position = position;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _isLaunched = true;
                }
            }
            else
            {
                MoveBall();
            }
        }

        private void LateUpdate()
        {
            _brickHitted = false;
        }

        private void MoveBall()
        {
            var nextPosition = _rigidBody.position + _direction.ToVector2() * (speed * Time.fixedDeltaTime);
            var overlapBox = Physics2D.OverlapBox(nextPosition, _collider2D.bounds.size.ToVector2() * 0.5f, 0, _layerMask);
            
            if (overlapBox != null)
            {
                _rigidBody.position = _rigidBody.position + (_direction.ToVector2() * overlapBox.Distance(_collider2D).distance);
                var newDirection = Vector2.Reflect(_direction, overlapBox.Distance(_collider2D).normal).normalized;
                speed = Mathf.Clamp(speed + 0.1f, 4, 8);

                switch (overlapBox.tag)
                {
                    case "Player":
                        var hit = Physics2D.Raycast(_rigidBody.position, _direction);
                        if (magnet)
                        {
                            _isLaunched = false;
                            _contactPointX = _paddle.transform.position.x - hit.point.x;
                        }
                        else
                        {
                            var center = overlapBox.bounds.center;
                            newDirection.x += center.x > hit.point.x ? -deviation : deviation;
                            _paddle.PaddlePunch();
                        }
                        break;
                    
                    case "Brick":
                        if (!_brickHitted)
                        {
                            _brickHitted = true;
                            var brick = overlapBox.GetComponent<Brick>();
                            brick.Hit();
                            if (superBall)
                            {
                                _brickHitted = false;
                                newDirection = _direction;
                            }
                        }
                        break;
                }
                MoveEffects();
                _direction = newDirection;
            }
            _rigidBody.MovePosition(_rigidBody.position + _direction.ToVector2().normalized * (speed * Time.fixedDeltaTime));
        }

        private void MoveEffects()
        {
            var angle = Mathf.Atan2(_direction.x, _direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (_scaleTweener == null || !_scaleTweener.active)
            {
                _scaleTweener = _sprite.DOPunchScale(Vector3.one * 0.25f, 0.15f).SetEase(Ease.InExpo).SetAutoKill();
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag("DeadZone")) return;
            if (OnBallDestroyed == null) return;
            OnBallDestroyed.Invoke();
            OnBallDestroyed -= GameManager.Instance.ArkanoidManager.OnBallDestroyed;
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            speed = 5;
            _isLaunched = false;
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

        public void InstantiateBall(Vector3 postion)
        {
            BallController ball = PoollingPrefabManager.Instance.GetPooledPrefab(gameObject, postion).GetComponent<BallController>();
            ball._isLaunched = true;
            ball._direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        protected override void OnGameStateChanged(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Playing:
                    _move = true;
                    break;
                case GameManager.GameState.PlayerDead:
                    //DOTween.KillAll();
                    gameObject.SetActive(false);
                    break;
                case GameManager.GameState.GameOver:
                    gameObject.SetActive(false);
                    break;
            }
        }
        
        #endregion
    }
}