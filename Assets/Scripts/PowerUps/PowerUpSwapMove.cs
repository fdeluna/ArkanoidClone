
namespace PowerUps
{
    public class PowerUpSwapMove : PowerUp
    {            
        public override void ApplyPowerUp()
        {
            Paddle.SwapControls(true);
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Paddle.SwapControls(false);
        }
    }
}
