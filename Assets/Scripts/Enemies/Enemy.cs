using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        public static UnityEvent<Enemy> EnemyStop = new();
        public static UnityEvent<int> PlayerLostLife = new();
        
        private int _health;
        private int _damage;
        private float _speed;
        private SpriteRenderer _spriteRenderer;
        private static GameObject _enemyPrefab;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public static Enemy InstantiateEnemy(Vector3 position, int health, int damage, Sprite sprite, float speed = 10)
        {
            var enemy = Instantiate(_enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
            enemy.Initialize(health, damage, sprite, speed);

            return enemy;
        }

        private void Initialize(int health, int damage, Sprite sprite, float speed)
        {
            _health = health;
            _damage = damage;
            _spriteRenderer.sprite = sprite;
            _speed = speed;
        }
        
        public void TakeDamage(int damage)
        {
            _health -= damage;

            if (_health > 0) return;
            
            EnemyStop.Invoke(this);
        }
        
        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(1f);
            // ToDo: Animation Stuff, Color blips, whatever
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("EnemyGoal")) return;

            EnemyStop.Invoke(this);
            PlayerLostLife.Invoke(_damage);
        }

        private IEnumerator HandelGoalReached()
        {
            yield return new WaitForSeconds(1f);
            // ToDo: Short animation stuff and fade
            Destroy(gameObject);
        }
    }
}