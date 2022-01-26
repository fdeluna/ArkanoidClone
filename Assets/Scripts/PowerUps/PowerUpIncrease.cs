namespace PowerUps
{
    public class PowerUpIncrease : PowerUpGrow
    {
        public  override void ApplyPowerUp()
        {
            GrowType = Grow.Increase;
            base.ApplyPowerUp();                  
        }    
    }
}