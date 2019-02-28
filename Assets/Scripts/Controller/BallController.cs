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
    private Collider2D _collider2D;
    private bool _brickHitted = false;
    private int _layerMask;


    private Vector3 _contactPoint = Vector3.zero;
    private Vector2 nextPosition;

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
            RepositionRigidBody();
        }
    }

    private void LateUpdate()
    {
        _brickHitted = false;
    }

    private void RepositionRigidBody()
    {
        nextPosition = _rigidBody.position + (_direction.ToVector2() * speed) * Time.fixedDeltaTime;
        Collider2D collider = Physics2D.OverlapBox(nextPosition, _collider2D.bounds.size.ToVector2() / 2, 0, _layerMask);
        if (collider != null && !_brickHitted)
        {
            _brickHitted = true;
            _rigidBody.position = _rigidBody.position + (_direction.ToVector2() * collider.Distance(_collider2D).distance);
            Vector2 newDirection = Vector2.Reflect(_direction, collider.Distance(_collider2D).normal).normalized;
            speed = Mathf.Clamp(speed + 0.1f, 5, 10);

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
                        _brickHitted = false;
                    }
                    break;
            }
            _direction = newDirection;
        }
        _rigidBody.MovePosition(_rigidBody.position + (_direction.ToVector2().normalized * speed) * Time.fixedDeltaTime);
    }


    // TODO MOVE TO REPOSITIO RIGIDBODY
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (IsLaunched && !_brickHitted)
        //{
        //    _brickHitted = true;
        //    ContactPoint2D contactPoint = collision.contacts[0];
        //    Vector3 newDirection = Vector3.Reflect(_direction, contactPoint.normal).normalized;
        //    Debug.DrawLine(contactPoint.point, contactPoint.point + contactPoint.normal * 2, Color.white);
        //    Debug.DrawLine(contactPoint.point, contactPoint.point + newDirection.ToVector2() * 2, Color.blue);
        //    //speed = Mathf.Clamp(speed + 0.25f, 5, 10);

        //    switch (collision.collider.tag)
        //    {
        //        case "Player":
        //            if (Magnet)
        //            {
        //                IsLaunched = false;
        //                _contactPointX = _paddle.transform.position.x - contactPoint.point.x;
        //            }
        //            else
        //            {
        //                Vector3 center = collision.collider.bounds.center;
        //                newDirection.x += center.x > contactPoint.point.x ? -deviation : deviation;
        //            }
        //            break;
        //        case "Brick":
        //            Brick brick = collision.collider.GetComponent<Brick>();
        //            brick.Hit();
        //            if (IsSuperBall)
        //            {
        //                newDirection = _direction;
        //                _brickHitted = false;
        //            }
        //            break;
        //    }
        //    _direction = newDirection.normalized;
        //}
        //RepositionRigidBody();
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

    private void OnDrawGizmos()
    {
        if (_collider2D != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_contactPoint, 0.5f);
            Gizmos.DrawWireCube(nextPosition, _collider2D.bounds.size);
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