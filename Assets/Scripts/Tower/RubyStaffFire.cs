using DG.Tweening;
using UnityEngine;

namespace Tower
{
    public class RubyStaffFire : MonoBehaviour
    {
        private const int _lifeTime = 4;
        private float _timer;
        private int _damage;
        private bool _isDead;


        public void Initialize(int damage)
        {
            _damage = damage;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            
            if (_timer < _lifeTime || _isDead) return;
            
            _isDead = true;
            transform
                .GetComponent<SpriteRenderer>()
                .DOFade(0, 1)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(_damage);
            }
        }
    }
}