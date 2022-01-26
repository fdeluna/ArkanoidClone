using PowerUps;
using System;
using UnityEngine;

namespace Level
{
    [Serializable]
    public class PowerUpProbability
    {
        public PowerUp powerUp;
        [Range(0,1)]
        public float probability = 0;   
    }
}
