using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSuperBall : PowerUp
{
    public override void ApplyPowerUp()
    {
        _ball.IsSuperBall = true;
    }
}
