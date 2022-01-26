namespace PowerUps
{
    public class PowerUpDecrease : PowerUpGrow
    {
        public override void ApplyPowerUp()
        {
            GrowType = Grow.Decrease;
            base.ApplyPowerUp();
        }
    }
}