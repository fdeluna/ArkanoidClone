using Manager;

namespace PowerUps
{
    public class PowerUpFreezeTime : PowerUp
    {
        public override void ApplyPowerUp()
        {
            ArkanoidManager.Instance.FreezeTime(true);
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            ArkanoidManager.Instance.FreezeTime(false);
        }
    }
}
