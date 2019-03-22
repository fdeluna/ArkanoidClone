using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField]
    float speed = 2.5f;
    [SerializeField]
    float fireRate = 0.35f;
    [SerializeField]
    GameObject proyectile;

    [HideInInspector]
    public Vector3 InitScale;
    [HideInInspector]
    public PowerUp CurrentPowerUp;


    private Rigidbody2D _rigidBody;
    private bool _fire = false;
    private float _randomnessMove = 0;
    private Vector3 _initPosition;
    private Tweener _currentTween;

    private Transform _gun;
    private Transform _sprite;

    private void Awake()
    {
        _initPosition = transform.position;
        _gun = transform.Find("Gun");
        _sprite = transform.Find("Sprite");
        _rigidBody = GetComponent<Rigidbody2D>();
        InitScale = transform.localScale;
    }

    void FixedUpdate()
    {
        // TODO CHANGE TO MOUSE
        Vector2 direction = Vector3.right * Input.GetAxis("Horizontal");
        Vector2 paddlePos = transform.position.ToVector2() + direction * speed * Time.deltaTime;
        paddlePos.x += Random.Range(-_randomnessMove, _randomnessMove);
        transform.position = paddlePos;
        //_rigidBody.MovePosition(paddlePos);
    }

    public void Reset()
    {
        transform.position = _initPosition;
        transform.localScale = InitScale;
        DisableGun();
        _randomnessMove = 0;
    }

    public void PaddlePunch()
    {
        _sprite.DOPunchPosition(Vector3.down, 0.3f).SetEase(Ease.InOutExpo).OnComplete(() => _sprite.localPosition = Vector3.zero);
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

    public void EnableGun()
    {
        _gun.gameObject.SetActive(true);
        _gun.DOLocalMoveY(1, 0.75f).SetEase(Ease.OutBounce).OnComplete(() => StartCoroutine(FireGun()));
    }

    public void DisableGun()
    {
        if (_fire)
        {
            _gun.DOLocalMoveY(0, 0.75f).SetEase(Ease.OutBounce).SetAutoKill(true).OnComplete(() => _gun.gameObject.SetActive(false));
            _fire = false;
        }
    }

    IEnumerator FireGun()
    {
        _fire = true;
        while (_fire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (Transform firePoint in _gun)
                {
                    PoollingPrefabManager.Instance.GetPooledPrefab(proyectile, firePoint.position);
                }
                yield return new WaitForSeconds(fireRate);
            }
            yield return null;
        }
    }

    #endregion
}