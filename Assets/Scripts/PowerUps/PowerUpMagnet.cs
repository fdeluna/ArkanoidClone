namespace PowerUps
{
    public class PowerUpMagnet : PowerUp
    {
        protected override void ApplyPowerUp()
        {
            Ball.magnet = true;
        }
    }
}