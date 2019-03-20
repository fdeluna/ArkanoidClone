public class PowerUpIncrease : PowerUpGrow
{    
    public override void ApplyPowerUp()
    {
        _growType = Grow.Increase;
        base.ApplyPowerUp();                  
    }    
}