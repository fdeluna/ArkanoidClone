public class PowerUpGrow : PowerUp
{
    public enum Grow {Increase,Decrease};
    public Grow growType;
    public float IncreaseMultiply = 2;

    public override void ApplyPowerUp()
    {        
        _paddle.ModifyScale(growType == Grow.Increase ? _paddle.InitScale * IncreaseMultiply : _paddle.InitScale / IncreaseMultiply);
    }    
}