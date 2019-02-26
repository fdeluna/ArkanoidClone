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
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _collider.enabled = true;

        // Needed here due to there could be more than one ballController for MultiBallPowerUp
        foreach (BallController ball in FindObjectsOfType<BallController>())
        {
            if (ball.gameObject.activeInHierarchy)
            {
                _ball = ball;
                break;
            }
        }
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
