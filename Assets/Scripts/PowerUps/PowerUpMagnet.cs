public class PowerUpMagnet : PowerUp
{
    public override void ApplyPowerUp()
    {
        _ball.Magnet = true;
    }
}