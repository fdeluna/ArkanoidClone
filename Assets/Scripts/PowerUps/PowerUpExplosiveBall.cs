
namespace PowerUps
{
    public class PowerUpExplosiveBall : PowerUp
    {            
        public override void ApplyPowerUp()
        {
            Ball.explosiveBall = true;
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Ball.explosiveBall = false;
        }
    }
}
