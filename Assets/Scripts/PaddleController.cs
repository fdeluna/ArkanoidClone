using DG.Tweening;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] float speed = 2.5f;

    public Vector3 InitScale;

    private PowerUp _powerUp;

    private Vector3 _initPosition;
    private Tweener _currentTween;

    private void Awake()
    {
        _initPosition = transform.position;
        InitScale = transform.localScale;
    }

    void FixedUpdate()
    {
        // TODO CHANGE TO MOUSE
        Vector3 direction = Vector3.right * Input.GetAxis("Horizontal");
        Vector2 paddlePos = transform.position + direction * speed * Time.deltaTime;
        transform.position = paddlePos;
    }

    public void Reset()
    {
        transform.position = _initPosition;
        transform.localScale = InitScale;
    }

    public void ModifyScale(Vector3 scale)
    {
        _currentTween?.Kill();
        transform.DOShakeScale(0.25f, 2.5f).SetAutoKill(true);
        _currentTween = transform.DOScale(new Vector3(scale.x, InitScale.y), 0.35f).SetAutoKill(true);
    }
}