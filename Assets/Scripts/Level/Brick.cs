using UnityEngine;

public class Brick : MonoBehaviour
{
    public enum BrickType { DESTRUCTIBLE, INDESTRUTIBLE }

    public BrickType bricktype = BrickType.DESTRUCTIBLE;
    [SerializeField] int hitsToDestroy = 1;

    public delegate void BrickDestroyed(Brick brick);
    public event BrickDestroyed OnBrickDestroyed;

    public void Hit()
    {
        if (bricktype == BrickType.DESTRUCTIBLE)
        {
            hitsToDestroy--;

            if (hitsToDestroy <= 0)
            {
                Destroy();
            }
        }
    }

    void Destroy()
    {
        OnBrickDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }
}