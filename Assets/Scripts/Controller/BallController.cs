using UnityEngine;

public class BallController : MonoBehaviour
{
    [Range(5, 10)]
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
    private bool _brickHitted = false;

    public delegate void BallDestroyed();
    public event BallDestroyed OnBallDestroyed;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _paddle = FindObjectOfType<PaddleController>();
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
            _rigidBody.MovePosition(_rigidBody.position + (_direction.ToVector2() * speed) * Time.fixedDeltaTime);            
        }
    }

    private void LateUpdate()
    {
        _brickHitted = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsLaunched && !_brickHitted)
        {
            _brickHitted = true;
            ContactPoint2D contactPoint = collision.contacts[0];
            Vector3 newDirection = Vector3.Reflect(_direction, contactPoint.normal).normalized;
            Debug.DrawLine(contactPoint.point, contactPoint.point + contactPoint.normal * 2, Color.white);
            Debug.DrawLine(contactPoint.point, contactPoint.point + newDirection.ToVector2() * 2, Color.blue);
            //speed = Mathf.Clamp(speed + 0.25f, 5, 10);

            switch (collision.collider.tag)
            {
                case "Player":
                    if (Magnet)
                    {
                        IsLaunched = false;
                        _contactPointX = _paddle.transform.position.x - contactPoint.point.x;
                    }
                    else
                    {
                        Vector3 center = collision.collider.bounds.center;
                        newDirection.x += center.x > contactPoint.point.x ? -deviation : deviation;
                    }
                    break;
                case "Brick":
                    Brick brick = collision.collider.GetComponent<Brick>();
                    brick.Hit();
                    if (IsSuperBall)
                    {
                        newDirection = _direction;
                        _brickHitted = false;
                    }
                    break;
            }
            _direction = newDirection;
        }
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