﻿using UnityEngine;

public class BallController : MonoBehaviour
{
    [Range(5, 10)]
    [SerializeField] float speed;
    [SerializeField] float deviation = 0.3f;

    [HideInInspector]
    public bool Magnet = false;
    public bool IsLaunched = false;

    private Vector3 _direction = new Vector2(0.15f, 1f);
    private float _contactPointX = 0;
    private PaddleController _paddle;

    public delegate void BallDestroyed();
    public event BallDestroyed OnBallDestroyed;

    private void Awake()
    {
        _paddle = FindObjectOfType<PaddleController>();
    }

    void FixedUpdate()
    {
        if (!IsLaunched)
        {
            Vector3 position = !Magnet ? new Vector3(_paddle.transform.position.x, _paddle.transform.position.y + 0.5f) : new Vector3(_paddle.transform.position.x - _contactPointX, _paddle.transform.position.y + 0.5f);
            transform.position = position;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsLaunched = true;
            }
        }
        else
        {
            transform.position += _direction.normalized * speed * Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsLaunched)
        {
            Vector3 center = collision.collider.bounds.center;
            ContactPoint2D contactPoint = collision.contacts[0];

            _contactPointX = _paddle.transform.position.x - contactPoint.point.x;
            _direction = Vector3.Reflect(_direction, contactPoint.normal);

            switch (collision.collider.tag)
            {
                case "Player":
                    if (Magnet)
                    {
                        IsLaunched = false;
                    }
                    else
                    {
                        _direction.x += center.x > contactPoint.point.x ? -deviation : deviation;
                        speed = Mathf.Clamp(speed + 0.25f, 5, 10);
                    }
                    break;
                case "Brick":
                    Brick brick = collision.collider.GetComponent<Brick>();
                    brick.Hit();
                    break;
            }
        }
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

    public void Reset()
    {
        speed = 5;
        IsLaunched = false;
        _direction = new Vector2(0.15f, 1f);
        Magnet = false;
    }

    public void ResetPowerUps()
    {
        Magnet = false;
    }

    public void InstantiateBall()
    {
        BallController ball = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<BallController>();
        ball.IsLaunched = true;
        ball._direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        //ball.speed = speed * 2;
        Debug.Log(ball._direction);
    }
}