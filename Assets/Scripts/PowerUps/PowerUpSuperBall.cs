namespace PowerUps
{
    public class PowerUpSuperBall : PowerUp
    {
        protected override void ApplyPowerUp()
        {
            Ball.superBall = true;
        }
    }
}
