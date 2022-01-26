using DG.Tweening;
using Manager;
using UnityEngine;

namespace Level
{
    public class Brick : MonoBehaviour
    {
        public enum BrickType { Destructible, Indestrutible }

        public BrickType brickType = BrickType.Destructible;
        [SerializeField] int hitsToDestroy = 1;

        public GameObject BrickParticles;

        public delegate void BrickDestroyed(Brick brick);
        public event BrickDestroyed OnBrickDestroyed;

        private Tweener _currentTween;
        private Vector3 _initPosition;
        private SpriteRenderer _sprite;
        private void Awake()
        {             
            _sprite = GetComponent<SpriteRenderer>();
        }

        // spawnDelay min max delay
        public Tween SpawnPosition(Vector3 startPosition, Vector3 endPosition, Vector2 spawnDelay)
        {            
            transform.localPosition = startPosition;
            _initPosition = endPosition;
            return transform.DOMove(endPosition, 1f).SetDelay(Random.Range(spawnDelay.x, spawnDelay.y)).SetEase(Ease.OutExpo).OnComplete(() => transform.position = endPosition);
        }

        public Tween SpawnScale(Vector3 endScale)
        {            
            transform.localScale = Vector3.zero;
            _initPosition = transform.position;
            return transform.DOScale(endScale, 1f).SetDelay(Random.Range(0.5f, 1.5f)).SetEase(Ease.OutElastic).OnComplete(() => transform.localScale = endScale);
        }


        public void Hit()
        {
            if (brickType != BrickType.Destructible) return;
            hitsToDestroy--;
            if (hitsToDestroy <= 0)
            {
                ParticleSystem.MainModule particleMM = PoollingPrefabManager.Instance.GetPooledPrefab(BrickParticles, transform.position).GetComponent<ParticleSystem>().main;
                particleMM.startColor = _sprite.color;
                Destroy();
            }
        }

        private void Destroy()
        {
            OnBrickDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }

        public void HitAnimation(float delay, Vector3 direction)
        {            
            if (_currentTween == null || !_currentTween.IsActive())
            {
                _currentTween = transform.DOLocalMove(transform.localPosition + (direction / 7f), 0.1f).SetDelay(delay)
                    .SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuint).OnComplete(() => transform.localPosition = _initPosition);
            }
        }
    }
}