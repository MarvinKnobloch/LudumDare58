using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Enemies;
using JetBrains.Annotations;
using Vector2 = UnityEngine.Vector2;

namespace Tower
{
    public class TowerBase : MonoBehaviour
    {
        public static UnityEvent<TowerBase, BodyObject> BodyPartEquipped { get; set; } = new();
        public static UnityEvent<TowerBase, BodyObject> BodyPartUnequipped { get; set; } = new();

        [SerializeField] private readonly float _baseAttackSpeed;
        [SerializeField] private readonly float _baseDamage;
        [SerializeField] private readonly float _baseRange;
        [SerializeField] private float _aoeRadius = 0;

        private float _currentAttackSpeed;
        private float _currentDamage;
        private float _currentRange;
        private Transform _transform;
        private AttackAnimationType _animationType = AttackAnimationType.None;

        public readonly List<BodyObject> EquippedBodyParts = new();

        private void Awake()
        {
            _currentRange = _baseRange;
            _currentDamage = _baseDamage;
            _currentAttackSpeed = _baseAttackSpeed;
            _transform = transform;
        }

        private void OnEnable()
        {
            BodyPartEquipped.AddListener(OnBodyPartEquipped);
            BodyPartUnequipped.AddListener(OnBodyPartUnequipped);
        }

        private void OnDisable()
        {
            BodyPartEquipped.RemoveListener(OnBodyPartEquipped);
            BodyPartUnequipped.RemoveListener(OnBodyPartUnequipped);
        }

        private void OnBodyPartEquipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyParts.Add(bodyObject);
            _currentAttackSpeed += bodyObject.AttackSpeedModifier;
            _currentDamage += bodyObject.DamageModifier;
            _currentRange += bodyObject.RangeModifier;
            _aoeRadius = bodyObject.AoeRadius;
            
            if (bodyObject.Part == BodyPart.Arm)
                _animationType = bodyObject.AnimationType;
        }

        private void OnBodyPartUnequipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyParts.Remove(bodyObject);
            _currentAttackSpeed -= bodyObject.AttackSpeedModifier;
            _currentDamage -= bodyObject.DamageModifier;
            _currentRange = bodyObject.RangeModifier;
            _aoeRadius = 0;
            
            if (bodyObject.Part == BodyPart.Arm)
                _animationType = AttackAnimationType.None;
        }

        private bool Attack()
        {
            var enemy = GetClosestEnemy();

            if (enemy == null || _animationType == AttackAnimationType.None) return false;

            if (Vector2.Distance(_transform.position, enemy.position) > _currentRange) return false;

            switch (_animationType)
            {
                case AttackAnimationType.AoeMelee:
                case AttackAnimationType.SingleMelee:
                    HandleMeleeAttack(enemy);
                    break;
                case AttackAnimationType.AoeRanged:
                case AttackAnimationType.SingleRanged:
                    HandleRangedAttack(enemy);
                    break;
                case AttackAnimationType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return true;
        }

        [CanBeNull]
        private Transform GetClosestEnemy()
        {
            var enemies = GameObject
                .FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Select(e => e.transform)
                .ToList();

            if (enemies.Count == 0) return null;

            var closestEnemy = enemies.First();
            var minDistance = float.MaxValue;
            
            foreach (var enemy in enemies)
            {
                var distance = Vector2.Distance(_transform.position, enemy.position);
                
                if (!(distance < minDistance)) continue;
                
                minDistance = distance;
                closestEnemy = enemy;
            }

            return closestEnemy;
        }

        private void HandleMeleeAttack(Transform target)
        {
            // ToDo
            // Aoe:
            // OverlapCircle with tower as center
            // Single:
            // Attack Animation on top of target
            // Both with some kind of faked tower animation/ color blip
            
            // Or switch by weapon type
        }

        private void HandleRangedAttack(Transform target)
        {
            // ToDo
            // Particle flies to enemy, then
            // Aoe:
            // OverlapCircle with enemy as center
            // Single:
            // Attack Animation on top of target
            // Both with some kind of faked tower animation/ color blip
            
            // or switch by weapon type
        }
    }
}