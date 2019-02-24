public class PowerUpGrow : PowerUp
{
    public float IncreaseMultiply = 2;
    protected enum Grow {Increase,Decrease};
    protected Grow _growType;

    public override void ApplyPowerUp()
    {        
        _paddle.ModifyScale(_growType == Grow.Increase ? _paddle.InitScale * IncreaseMultiply : _paddle.InitScale / IncreaseMultiply);
    }    
}