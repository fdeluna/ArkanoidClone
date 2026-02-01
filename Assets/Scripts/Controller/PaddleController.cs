using System.Collections;
using DG.Tweening;
using Manager;
using UnityEngine;

namespace Controller
{
    public class PaddleController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        private Vector2 Limits;

        [Header("Ball")]
        [SerializeField]
        private GameObject ball;
        [Header("Gun")]
        [SerializeField]
        private float fireRate = 0.35f;
        [SerializeField]
        private GameObject projectile;
        [SerializeField]
        private ParticleSystem[] muzzleParticles;


        [HideInInspector]
        public Vector3 initScale;
        [HideInInspector]
        public bool move = false;

        private Rigidbody2D _rigidBody;
        private bool _fire = false;
        private float _swapControls = 1;
        private float _randomnessMove = 0;
        private Vector3 _initPosition;
        private Vector3 _currentScale;
        private Tweener _currentTween;

        private Transform _gun;
        private Tween _gunTween;
        private Transform _sprite;

        private void Start()
        {
            _initPosition = transform.position;
            _currentScale = initScale = transform.localScale;
            transform.localScale = Vector3.zero;
            _gun = transform.Find("Sprite/Gun");
            _sprite = transform.Find("Sprite");
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!move) return;
            Vector2 direction = Vector3.right * Input.GetAxis("Horizontal") * _swapControls;
            var paddlePos = transform.position.ToVector2() + direction * (speed * Time.deltaTime);
            paddlePos.x += Random.Range(-_randomnessMove, _randomnessMove);
            if (paddlePos.x < Limits.x && paddlePos.x > Limits.y)
            {
                transform.position = paddlePos;
            }
        }

        public void PaddleSpawn()
        {
            Reset();
            
            transform.localScale = Vector3.zero;            
            transform.DOScale(initScale, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
            {
                move = true;
            });

        }

        public BallController BallSpawn()
        {
            var ballPosition = transform.localPosition;
            ballPosition.y += 0.5f;
            return PoollingPrefabManager.Instance.GetPooledPrefab(ball, ballPosition).GetComponent<BallController>();
        }

        public void PaddleHide()
        {
            move = false;
            transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutElastic);
        }

        public void PaddleDead()
        {
            move = false;
            transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutElastic);
        }


        public void Reset()
        {
            transform.position = _initPosition;
            transform.localScale = initScale;
            DisableGun();
            _randomnessMove = 0;
        }

        public void PaddlePunch()
        {
            _sprite.DOPunchPosition(Vector3.down, 0.3f).SetEase(Ease.InOutExpo).OnComplete(() => _sprite.localPosition = Vector3.zero);
        }

        #region Power Ups

        // TODO RESET PADDLE
        public void ResetPowerUps()
        {
            _randomnessMove = 0;
            DisableGun();
            ModifyScale(initScale);
        }

        public void ModifyScale(Vector3 scale)
        {
            if (scale != transform.localScale)
            {

                var scaleDiff = Mathf.Abs(scale.x - _currentScale.x);

                if (scale.x > _currentScale.x)
                {
                    Limits.x -= scaleDiff;
                    Limits.y += scaleDiff;
                }
                else
                {
                    Limits.x += scaleDiff;
                    Limits.y -= scaleDiff;
                }

                Vector3 newPosition = transform.position;
                if (transform.position.x > Limits.x)
                {
                    newPosition.x -= Mathf.Abs(transform.position.x - Limits.x);
                    transform.position = newPosition;
                }
                else if (transform.position.x < Limits.y)
                {
                    newPosition.x += Mathf.Abs(transform.position.x - Limits.y);
                    transform.position = newPosition;
                }


                _currentTween?.Kill();
                transform.DOShakeScale(0.25f, 2.5f).SetAutoKill(true);
                _currentTween = transform.DOScale(new Vector3(scale.x, initScale.y), 0.35f);
                _currentScale = scale;
            }
        }

        public void ResetScale()
        {
            ModifyScale(initScale);
        }

        public void RandomMoves(float randomness = 0)
        {
            _randomnessMove = randomness;
        }

        public void SwapControls(bool swap)
        {
            _swapControls = swap ? -1 : 1;
        }

        public void EnableGun()
        {
            _gun.gameObject.SetActive(true);
            _gunTween?.Kill();
            _gunTween = _gun.DOLocalMoveY(1, 0.75f).SetEase(Ease.OutBounce).OnComplete(() => StartCoroutine(FireGun()));
        }

        public void DisableGun()
        {
            if (!_fire) return;
            _gunTween?.Kill();
            _gunTween =_gun.DOLocalMoveY(0, 0.75f).SetEase(Ease.OutBounce).SetAutoKill(true).OnComplete(() => _gun.gameObject.SetActive(false));
            _fire = false;
        }

        private IEnumerator FireGun()
        {
            _fire = true;
            while (_fire)
            {
                if (Input.GetKeyDown(KeyCode.Space) && move)
                {
                    _gun.DOPunchPosition(Vector3.down * 0.35f, 0.1f).SetEase(Ease.InOutExpo).OnComplete(() => _gun.localPosition = Vector3.up);
                    
                    foreach (Transform firePoint in _gun)
                    {
                        PoollingPrefabManager.Instance.GetPooledPrefab(projectile, firePoint.position);
                    }
                    
                    foreach (ParticleSystem emissionModule in muzzleParticles)
                    {
                        emissionModule.Play();
                    }
                    
                    yield return new WaitForSeconds(fireRate);
                }
                yield return null;
            }
        }

        #endregion
    }
}