using UnityEngine;

namespace Level
{
    public class Brick : MonoBehaviour
    {
        public enum BrickType { Destructible, Indestrutible }

        public BrickType brickType = BrickType.Destructible;
        [SerializeField] int hitsToDestroy = 1;

        public delegate void BrickDestroyed(Brick brick);
        public event BrickDestroyed OnBrickDestroyed;

        public void Hit()
        {
            if (brickType != BrickType.Destructible) return;
            hitsToDestroy--;
            if (hitsToDestroy <= 0)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            OnBrickDestroyed?.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}