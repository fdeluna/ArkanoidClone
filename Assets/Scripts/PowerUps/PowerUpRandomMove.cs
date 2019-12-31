
namespace PowerUps
{
    public class PowerUpRandomMove : PowerUp
    {    
        public float randomnessStrength = 0.15f;

        protected override void ApplyPowerUp()
        {
            Paddle.RandomMoves(randomnessStrength);
        }
    }
}
