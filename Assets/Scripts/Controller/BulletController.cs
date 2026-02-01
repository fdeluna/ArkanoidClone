using Level;
using UnityEngine;

namespace Controller
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField]
        private float speed = 4f;
        [SerializeField]
        private GameObject particles;

        private void FixedUpdate()
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // TODO SPAWN PARTICLES
            Destroy(gameObject);
            var brick = collision.collider.GetComponent<Brick>();
            if (brick) brick.Hit(true);
        }
    }
}
