public class PowerUpDecrease : PowerUpGrow
{
    public override void ApplyPowerUp()
    {
        _growType = Grow.Decrease;
        base.ApplyPowerUp();
    }
}