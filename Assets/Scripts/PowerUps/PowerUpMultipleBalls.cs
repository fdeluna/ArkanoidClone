

public class PowerUpMultipleBalls : PowerUp
{
    public int balls = 5;

    public override void ApplyPowerUp()
    {
        for (int i =0; i < balls; i++)
        {
            _ball.InstantiateBall();
        }
    }
}
