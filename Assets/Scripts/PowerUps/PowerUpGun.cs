public class PowerUpGun : PowerUp
{   
    public override void ApplyPowerUp()
    {
        _paddle.EnableGun();
    }
}
