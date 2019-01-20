using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpRandomMove : PowerUp
{    
    public float RandomnessStrength = 0.15f;
    private PaddleController _paddle;

    private void Awake()
    {
        _paddle = FindObjectOfType<PaddleController>();
    }

    public override void ApplyPowerUp()
    {
        _paddle.RandomMoves(RandomnessStrength);
    }
}
