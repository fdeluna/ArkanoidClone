using System;
using UnityEngine;

[Serializable]
public class PowerUpProbability
{
    public GameObject powerUp;
    [Range(0,1)]
    public float probability = 0;   
}
