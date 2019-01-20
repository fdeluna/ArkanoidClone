using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public GameObject particles;

    protected PaddleController _paddle;
    protected BallController _ball;

    private void Awake()
    {
        _paddle = FindObjectOfType<PaddleController>();
        _ball = FindObjectOfType<BallController>();
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        _paddle.ResetPowerUps();
        _ball.ResetPowerUps();
        // TODO POOL PARTICLES
        Destroy(gameObject);
        ApplyPowerUp();
        // TODO POOL DISABLE
    }

    public abstract void ApplyPowerUp();
}
