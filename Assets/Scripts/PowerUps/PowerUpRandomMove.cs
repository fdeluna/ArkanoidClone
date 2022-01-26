
namespace PowerUps
{
    public class PowerUpRandomMove : PowerUp
    {    
        public float randomnessStrength = 0.15f;

        public override void ApplyPowerUp()
        {
            Paddle.RandomMoves(randomnessStrength);
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Paddle.RandomMoves();
        }
    }
}
