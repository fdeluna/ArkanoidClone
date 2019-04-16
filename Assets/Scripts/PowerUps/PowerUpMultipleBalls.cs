public class PowerUpMultipleBalls : PowerUp
{
    public int balls = 5;

    public override void ApplyPowerUp()
    {
        GameManager.Instance.ArkanoidManager.TotalBalls += 5;
        for (int i = 0; i < balls; i++)
        {
            _ball.InstantiateBall(_ball.transform.position);
        }
    }
}
