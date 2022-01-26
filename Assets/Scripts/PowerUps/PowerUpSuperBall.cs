namespace PowerUps
{
    public class PowerUpSuperBall : PowerUp
    {
        public override void ApplyPowerUp()
        {
            Ball.superBall = true;
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Ball.superBall = false;
        }
    }
}
