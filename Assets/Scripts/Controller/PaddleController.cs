using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PaddleController : ArkanoidObject
{
    [Header("Stats")]
    [SerializeField]
    float speed = 5f;
    public int Lifes = 3;

    [Header("Ball")]
    [SerializeField]
    GameObject Ball;
    [Header("Gun")]
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
    private bool _move = false;
    private float _randomnessMove = 0;
    private Vector3 _initPosition;
    private Tweener _currentTween;

    private Transform _gun;
    private Transform _sprite;

    private void Start()
    {
        _initPosition = transform.position;
        InitScale = transform.localScale;
        transform.localScale = Vector3.zero;
        _gun = transform.Find("Gun");
        _sprite = transform.Find("Sprite");
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_move)
        {
            Vector2 direction = Vector3.right * Input.GetAxis("Horizontal");
            Vector2 paddlePos = transform.position.ToVector2() + direction * speed * Time.deltaTime;
            paddlePos.x += Random.Range(-_randomnessMove, _randomnessMove);
            transform.position = paddlePos;
            //_rigidBody.MovePosition(paddlePos);
        }
    }

    public void PaddleSpawn()
    {
        Reset();
        transform.localScale = Vector3.zero;
        transform.DOScale(InitScale, 1f).SetEase(Ease.InOutElastic).SetDelay(1f);
        Vector3 ballPosition = transform.localPosition;
        ballPosition.y += 0.5f;
        PoollingPrefabManager.Instance.GetPooledPrefab(Ball, ballPosition);
    }

    public void PaddleDead()
    {
        _move = false;
        // TODO DESTROY EFFECT
        // AFTER EFFECTS
        transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            if (Lifes > 0)
            {
                PaddleSpawn();
                GameManager.Instance.CurrentState = GameManager.GameState.Playing;
            }
            else
            {
                GameManager.Instance.CurrentState = GameManager.GameState.GameOver;
            }
        });
        transform.localScale = Vector3.zero;

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


    protected override void OnGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Start:
                gameObject.SetActive(false);
                break;
            case GameManager.GameState.LoadGame:
                gameObject.SetActive(true);
                PaddleSpawn();
                break;
            case GameManager.GameState.Playing:
                _move = true;
                break;
            case GameManager.GameState.PlayerDead:
                PaddleDead();
                break;
            case GameManager.GameState.GameOver:
                gameObject.SetActive(false);
                GameManager.Instance.CurrentState = GameManager.GameState.Start;
                break;
            default:
                break;
        }
    }
}