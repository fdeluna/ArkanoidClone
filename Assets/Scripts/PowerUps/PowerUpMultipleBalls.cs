using Controller;
using System.Collections.Generic;
using Manager;

namespace PowerUps
{
    public class PowerUpMultipleBalls : PowerUp
    {
        public int MaxBalls = 5;
        private List<BallController> _balls = new List<BallController>();

        public override void ApplyPowerUp()
        {
            for (var i = 0; i < MaxBalls; i++)
            {
                _balls.Add(ArkanoidManager.Instance.ball.InstantiatePowerUpBall(ArkanoidManager.Instance.ball.transform
                    .position));
            }
        }

        public override void UnApplyPowerUp()
        {
            base.UnApplyPowerUp();
            _balls.ForEach(ball =>
            {
                if (ball.powerUpBall)
                {
                    ball.Destroy();
                }
            });
        }
    }
}