namespace PowerUps
{
    public class PowerUpGun : PowerUp
    {
        protected override void ApplyPowerUp()
        {
            Paddle.EnableGun();
        }
    }
}
