namespace PowerUps
{
    public class PowerUpMagnet : PowerUp
    {
        public override void ApplyPowerUp()
        {
            Ball.magnet = true;
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Ball.magnet = false;
        }
    }
}