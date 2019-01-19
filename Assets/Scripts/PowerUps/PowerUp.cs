using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public GameObject particles;
   

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO POOL PARTICLES
        Destroy(gameObject);        
        ApplyPowerUp();
        // TODO POOL DISABLE
    }

    public abstract void ApplyPowerUp();    
}
