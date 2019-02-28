using UnityEngine;

public class BallController : MonoBehaviour
{
    [Range(4, 8)]
    [SerializeField] float speed;
    [SerializeField] float deviation = 0.3f;

    [HideInInspector]
    public bool Magnet = false;
    [HideInInspector]
    public bool IsLaunched = false;
    [HideInInspector]
    public bool IsSuperBall = false;

    private Vector3 _direction = new Vector2(0.15f, 1f);
    private float _contactPointX = 0;
    private PaddleController _paddle;
    private Rigidbody2D _rigidBody;
    private Collider2D _collider2D;
    private bool _brickHitted = false;
    private int _layerMask;
    private Vector3 _contactPoint = Vector3.zero;


    public delegate void BallDestroyed();
    public event BallDestroyed OnBallDestroyed;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _paddle = FindObjectOfType<PaddleController>();
        _layerMask = (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Player"));
    }

    private void OnEnable()
    {
        OnBallDestroyed -= GameManager.Instance.LevelManager.OnBallDestroyed;
        OnBallDestroyed += GameManager.Instance.LevelManager.OnBallDestroyed;
    }

    void FixedUpdate()
    {
        if (!IsLaunched)
        {
            Vector3 position = !Magnet ? new Vector3(_paddle.transform.position.x, _paddle.transform.position.y + 0.5f) : new Vector3(_paddle.transform.position.x - _contactPointX, _paddle.transform.position.y + 0.5f);
            _rigidBody.position = position;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsLaunched = true;
            }
        }
        else
        {
            MoveBall();
        }
    }

    private void MoveBall()
    {
        Vector2 nextPosition = _rigidBody.position + (_direction.ToVector2() * speed) * Time.fixedDeltaTime;
        Collider2D collider = Physics2D.OverlapBox(nextPosition, _collider2D.bounds.size.ToVector2() * 0.5f, 0, _layerMask);
        if (collider != null)
        {
            _brickHitted = true;
            _rigidBody.position = _rigidBody.position + (_direction.ToVector2() * collider.Distance(_collider2D).distance);
            Vector2 newDirection = Vector2.Reflect(_direction, collider.Distance(_collider2D).normal).normalized;
            speed = Mathf.Clamp(speed + 0.1f, 4, 8);

            switch (collider.tag)
            {
                case "Player":
                    RaycastHit2D hit = Physics2D.Raycast(_rigidBody.position, _direction);
                    if (Magnet)
                    {
                        IsLaunched = false;
                        _contactPointX = _paddle.transform.position.x - hit.point.x;
                    }
                    else
                    {
                        Vector3 center = collider.bounds.center;
                        newDirection.x += center.x > hit.point.x ? -deviation : deviation;
                    }
                    break;
                case "Brick":
                    Brick brick = collider.GetComponent<Brick>();
                    brick.Hit();
                    if (IsSuperBall)
                    {
                        newDirection = _direction;
                    }
                    break;
            }
            _direction = newDirection;
        }
        _rigidBody.MovePosition(_rigidBody.position + (_direction.ToVector2().normalized * speed) * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "DeadZone")
        {
            if (OnBallDestroyed != null)
            {
                OnBallDestroyed.Invoke();
                OnBallDestroyed -= GameManager.Instance.LevelManager.OnBallDestroyed;
                gameObject.SetActive(false);
                Reset();
            }
        }
    }

    public void Reset()
    {
        speed = 5;
        IsLaunched = false;
        _direction = new Vector2(0.15f, 1f);
        ResetPowerUps();
    }

    #region Power Ups
    public void ResetPowerUps()
    {
        Magnet = false;
        IsSuperBall = false;
    }

    public void InstantiateBall(Vector3 postion)
    {
        BallController ball = PoollingPrefabManager.Instance.GetPooledPrefab(gameObject, postion).GetComponent<BallController>();
        ball.IsLaunched = true;
        ball._direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }


    #endregion
}