using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public GameObject particles;
    public float fallSpeed = 5;

    protected PaddleController _paddle;
    protected BallController _ball;

    private Collider2D _collider;

    private void Awake()
    {
        _paddle = FindObjectOfType<PaddleController>();
        _ball = FindObjectOfType<BallController>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _collider.enabled = true;
    }

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _paddle.CurrentPowerUp = this;        
        DestroyPowerUp();
    }

    public abstract void ApplyPowerUp();


    private void DestroyPowerUp()
    {
        _paddle.ResetPowerUps();
        _ball.ResetPowerUps();
        ApplyPowerUp();
        gameObject.SetActive(false);
    }
}
