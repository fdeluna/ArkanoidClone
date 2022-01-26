namespace PowerUps
{
    public class PowerUpGun : PowerUp
    {
        public override void ApplyPowerUp()
        {
            Paddle.EnableGun();
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Paddle.DisableGun();
        }

    }
}
