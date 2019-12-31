namespace PowerUps
{
    public class PowerUpMultipleBalls : PowerUp
    {
        public int balls = 5;

        protected override void ApplyPowerUp()
        {
            GameManager.Instance.ArkanoidManager.totalBalls += 5;
            for (var i = 0; i < balls; i++)
            {
                Ball.InstantiateBall(Ball.transform.position);
            }
        }
    }
}
