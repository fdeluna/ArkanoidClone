namespace PowerUps
{
    public class PowerUpDecrease : PowerUpGrow
    {
        protected override void ApplyPowerUp()
        {
            GrowType = Grow.Decrease;
            base.ApplyPowerUp();
        }
    }
}