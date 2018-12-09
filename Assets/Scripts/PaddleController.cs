using UnityEngine;

public class PaddleController : MonoBehaviour
{    
    [SerializeField] float speed = 2.5f;
    [SerializeField] float leftLimit = -6f;
    [SerializeField] float rightLimit = 2f;

    private Vector3 _initPosition;

    private void Awake()
    {
        _initPosition = transform.position;
    }

    void FixedUpdate()
    {
        // TODO CHANGE TO MOUSE
        Vector3 direction = Vector3.right * Input.GetAxis("Horizontal");
        Vector2 paddlePos = transform.position + direction * speed * Time.deltaTime;
        paddlePos.x = Mathf.Clamp(paddlePos.x, leftLimit, rightLimit);
        transform.position = paddlePos;
    }


    public void Reset()
    {
        transform.position = _initPosition;
    }
}
