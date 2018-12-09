using UnityEngine;

public class BallController : MonoBehaviour
{
    [Range(5, 10)]
    [SerializeField] float speed;
    [SerializeField] float deviation = 0.3f;

    private Vector3 _direction = new Vector2(0.15f, 1f);
    private bool _hitted = false;
    private bool _launched = false;
    private PaddleController _paddle;

    public delegate void BallDestroyed();
    public event BallDestroyed OnBallDestroyed;

    private void Awake()
    {
        _paddle = FindObjectOfType<PaddleController>();
    }

    void FixedUpdate()
    {
        if (!_launched)
        {
            Vector3 position = new Vector3(_paddle.transform.position.x, _paddle.transform.position.y + 0.5f);
            transform.position = position;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _launched = true;
            }
        }
        else
        {
            transform.position += _direction * speed * Time.fixedDeltaTime;
        }
    }

    void LateUpdate()
    {
        _hitted = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_hitted)
        {
            Vector3 center = collision.collider.bounds.center;
            ContactPoint2D contactPoint = collision.contacts[0];

            _direction = Vector3.Reflect(_direction, contactPoint.normal);

            switch (collision.collider.tag)
            {
                case "Player":
                    _direction.x += center.x > contactPoint.point.x ? -deviation : deviation;
                    speed = Mathf.Clamp(speed + 0.25f, 5, 10);
                    break;
                case "Brick":
                    Brick brick = collision.collider.GetComponent<Brick>();
                    _hitted = true;
                    brick.Hit();
                    break;
            }
        }
    }

    public void Reset()
    {
        speed = 5;
        _launched = false;
        _direction = new Vector2(0.15f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "DeadZone")
        {
            if (OnBallDestroyed != null)
            {
                OnBallDestroyed.Invoke();
            }
        }
    }
}