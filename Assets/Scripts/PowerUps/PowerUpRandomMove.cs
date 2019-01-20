
public class PowerUpRandomMove : PowerUp
{    
    public float RandomnessStrength = 0.15f;
        
    public override void ApplyPowerUp()
    {
        _paddle.RandomMoves(RandomnessStrength);
    }
}
