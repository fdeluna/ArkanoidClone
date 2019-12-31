namespace PowerUps
{
    public class PowerUpIncrease : PowerUpGrow
    {
        protected override void ApplyPowerUp()
        {
            GrowType = Grow.Increase;
            base.ApplyPowerUp();                  
        }    
    }
}