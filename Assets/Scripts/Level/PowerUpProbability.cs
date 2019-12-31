using System;
using UnityEngine;

namespace Level
{
    [Serializable]
    public class PowerUpProbability
    {
        public GameObject powerUp;
        [Range(0,1)]
        public float probability = 0;   
    }
}
