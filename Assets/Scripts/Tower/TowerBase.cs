using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using JetBrains.Annotations;
using Vector2 = UnityEngine.Vector2;

namespace Tower
{
    public class TowerBase : MonoBehaviour
    {
        public static UnityEvent<TowerBase, BodyObject> BodyPartEquipped { get; } = new();
        public static UnityEvent<TowerBase, BodyObject> BodyPartUnequipped { get; } = new();
        public static UnityEvent<List<Enemy>, int> AoeHit { get; } = new();
            
        [SerializeField] private readonly float _baseAttackSpeed;
        [SerializeField] private readonly float _baseDamage;
        [SerializeField] private readonly float _baseRange;
        [SerializeField] private float _aoeRadius = 0;

        private float _currentAttackSpeed;
        private float _currentDamage;
        private float _currentRange;
        private Transform _transform;
        private WeaponType _weaponType = WeaponType.None;

        public readonly List<BodyObject> EquippedBodyParts = new();

        private readonly Dictionary<WeaponType, float> _weaponStats = new()
        {
            {WeaponType.Boulder, 10},
            {WeaponType.IceStaff, 5},
            {WeaponType.Sword, 10},
            {WeaponType.Bow, 25},
            {WeaponType.Crossbow, 25},
            {WeaponType.Scroll, 10},
            {WeaponType.Club, 15},
            {WeaponType.Dagger, 20},
            {WeaponType.None, 0},
            {WeaponType.RubyStaff, 20},
            {WeaponType.CrystalStaff, 20},
            {WeaponType.Stone, 0},
        };

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
                _weaponType = bodyObject.Weapon;
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
                _weaponType = WeaponType.None;
        }

        private bool Attack()
        {
            var enemy = GetClosestEnemy();

            if (enemy == null || _weaponType == WeaponType.None) return false;

            if (Vector2.Distance(_transform.position, enemy.position) > _currentRange) return false;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (_weaponType)
            {
                case WeaponType.Boulder:
                case WeaponType.Bow:
                case WeaponType.Club:
                case WeaponType.Crossbow:
                case WeaponType.CrystalStaff:
                case WeaponType.Dagger:
                case WeaponType.IceStaff:
                case WeaponType.RubyStaff:
                case WeaponType.Scroll:
                case WeaponType.Stone:
                case WeaponType.Sword:
                    break;
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

        private int CalculateDamage()
        {
            return Mathf.RoundToInt(_weaponStats[_weaponType] * _currentDamage);
        }

        private void TriggerAoeHit(List<Enemy> enemies, int damage)
        {
            AoeHit.Invoke(enemies, damage);   
        }

        private void TriggerSingleHit(Enemy enemy, int damage)
        {
            enemy.HealthChange(damage);
        }
    }
}