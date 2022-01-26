namespace PowerUps
{
    public class PowerUpGrow : PowerUp
    {
        public float increaseMultiply = 2;
        protected enum Grow {Increase,Decrease};
        protected Grow GrowType;

        public override void ApplyPowerUp()
        {        
            Paddle.ModifyScale(GrowType == Grow.Increase ? Paddle.initScale * increaseMultiply : Paddle.initScale / increaseMultiply);
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            Paddle.ModifyScale(Paddle.initScale);
        }
    }
}