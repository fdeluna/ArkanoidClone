using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    float speed = 4f;
    [SerializeField]
    GameObject particles;
    
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO SPAWN PARTICLES
        Destroy(gameObject);
        Brick brick = collision.collider.GetComponent<Brick>();
        brick.Hit();
    }
}
