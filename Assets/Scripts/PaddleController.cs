using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PaddleController : MonoBehaviour
{

    public bool debug = false;

    [SerializeField]
    float speed = 2.5f;
    [SerializeField]
    float fireRate = 0.35f;

    [HideInInspector]
    public Vector3 InitScale;

    private bool _fire = false;
    private float _randomnessMove = 0;
    private Vector3 _initPosition;
    private Tweener _currentTween;

    private Transform _gun;

    private void Awake()
    {
        _initPosition = transform.position;
        _gun = transform.Find("Gun");
        InitScale = transform.localScale;
    }

    void FixedUpdate()
    {
        // TODO CHANGE TO MOUSE
        Vector3 direction = Vector3.right * Input.GetAxis("Horizontal");
        Vector2 paddlePos = transform.position + direction * speed * Time.deltaTime;
        paddlePos.x += Random.Range(-_randomnessMove, _randomnessMove);
        transform.position = paddlePos;
    }

    public void Reset()
    {
        transform.position = _initPosition;
        transform.localScale = InitScale;
        DisableGun();
        _randomnessMove = 0;
    }


    #region Power Ups

    public void ResetPowerUps()
    {
        _randomnessMove = 0;
        DisableGun();
        ModifyScale(InitScale);
    }

    public void ModifyScale(Vector3 scale)
    {
        if (scale != transform.localScale)
        {
            _currentTween?.Kill();
            transform.DOShakeScale(0.25f, 2.5f).SetAutoKill(true);
            _currentTween = transform.DOScale(new Vector3(scale.x, InitScale.y), 0.35f).SetAutoKill(true);
        }
    }

    public void ResetScale()
    {
        ModifyScale(InitScale);
    }

    public void RandomMoves(float randomness)
    {
        _randomnessMove = randomness;
    }

    // TODO SEGUIR POR AQUÍ
    public void EnableGun()
    {
        _gun.DOLocalMoveY(1, 0.75f).SetEase(Ease.OutBounce).OnComplete(() => StartCoroutine(FireGun()));
        _fire = true;
    }

    public void DisableGun()
    {
        if (_fire)
        {
            _gun.DOLocalMoveY(0, 0.75f).SetEase(Ease.OutBounce).SetAutoKill(true);
            _fire = false;
        }
    }

    IEnumerator FireGun()
    {
        while (_fire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("FIRE");
                yield return new WaitForSeconds(fireRate);
            }
            yield return null;
        }
    }
    #endregion
}