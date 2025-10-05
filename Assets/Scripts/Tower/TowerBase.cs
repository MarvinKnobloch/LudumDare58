using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;

namespace Tower
{
    public class TowerBase : MonoBehaviour
    {
        public static UnityEvent<TowerBase, BodyObject> BodyPartEquipped { get; } = new();
        public static UnityEvent<TowerBase, BodyObject> BodyPartUnequipped { get; } = new();
        public static UnityEvent<List<Enemy>, int, WeaponType> AoeHit { get; } = new();

        [SerializeField] private float _baseCooldown;
        [SerializeField] private float _baseDamage;
        [SerializeField] private float _baseRange;
        
        [SerializeField] private GameObject _projectilePrefab;

        private float _aoeRadius;
        private float _currentCooldown;
        private float _currentDamage;
        private float _currentRange;
        private Transform _transform;
        private WeaponType _weaponType = WeaponType.None;
        private BodyObject _weapon;
        private float _timer = 0;
        private Animator _animator;
        
        public List<BodyObject> EquippedBodyObjects = new();

        private readonly Dictionary<WeaponType, float> _weaponStats = new()
        {
            { WeaponType.Boulder, 10 },
            { WeaponType.IceStaff, 5 },
            { WeaponType.Sword, 10 },
            { WeaponType.Bow, 25 },
            { WeaponType.Crossbow, 25 },
            { WeaponType.Scroll, 10 },
            { WeaponType.Club, 15 },
            { WeaponType.Dagger, 20 },
            { WeaponType.None, 0 },
            { WeaponType.RubyStaff, 20 },
            { WeaponType.CrystalStaff, 20 },
            { WeaponType.Stone, 10 },
        };

        private void Awake()
        {
            _currentRange = _baseRange;
            _currentDamage = _baseDamage;
            _currentCooldown = _baseCooldown;
            _transform = transform;
            _animator = GetComponentInChildren<Animator>();

            var weaponObject = EquippedBodyObjects.FirstOrDefault(b => b.Weapon != WeaponType.None);

            if (!weaponObject) return;

            _weapon = weaponObject;
            _weaponType = weaponObject.Weapon;
            _aoeRadius = weaponObject.AoeRadius;
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

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < _currentCooldown) return;

            var attackPossible= Attack();
            Debug.Log(attackPossible);
            _timer = attackPossible ? 0 : _currentCooldown - 0.1f;
            Debug.Log(_timer);
        }

        private void OnBodyPartEquipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyObjects.Add(bodyObject);
            _currentCooldown *= bodyObject.AttackSpeedModifier > 0 ? bodyObject.AttackSpeedModifier : 1;
            _currentDamage += bodyObject.DamageModifier > 0 ? bodyObject.AttackSpeedModifier : 1;
            _currentRange += bodyObject.RangeModifier > 0 ? bodyObject.AttackSpeedModifier : 1;
            _aoeRadius = bodyObject.AoeRadius;

            if (bodyObject.Part == BodyPart.Arm)
                _weaponType = bodyObject.Weapon;
        }

        private void OnBodyPartUnequipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyObjects.Remove(bodyObject);
            _currentCooldown /= bodyObject.AttackSpeedModifier;
            _currentDamage -= bodyObject.DamageModifier;
            _currentRange = bodyObject.RangeModifier;
            _aoeRadius = 0;

            if (bodyObject.Part == BodyPart.Arm)
                _weaponType = WeaponType.None;
        }

        private bool Attack()
        {
            //Debug.Log("Attack!");
            var enemy = GetClosestEnemy();
            //Debug.Log(enemy.position);

            if (enemy == null || _weaponType == WeaponType.None) return false;

            if (Vector2.Distance(_transform.position, enemy.position) > _currentRange) return false;

            var damage = CalculateDamage();

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (_weaponType)
            {
                case WeaponType.Boulder:
                case WeaponType.Crossbow:
                case WeaponType.IceStaff:
                case WeaponType.Scroll:
                case WeaponType.Stone:
                    HandleRangedStandardAttack(damage, enemy);
                    break;
                case WeaponType.Club:
                case WeaponType.Dagger:
                case WeaponType.Sword:
                    HandleMeleeStandardAttack(damage, enemy);
                    break;
                case WeaponType.CrystalStaff:
                case WeaponType.RubyStaff:
                    break;
            }

            return true;
        }

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

        private List<Enemy> GetAoeEnemies(Transform origin)
        {
            var colliders = Physics2D.OverlapCircleAll(origin.position, _aoeRadius);

            return colliders
                .Select(c => c.TryGetComponent<Enemy>(out var enemy) ? enemy : null)
                .Where(e => e != null)
                .ToList();
        }

        private int CalculateDamage()
        {
            return Mathf.RoundToInt(_weaponStats[_weaponType] * _currentDamage);
        }

        private void HandleMeleeStandardAttack(int damage, Transform targetEnemy)
        {
            if (_aoeRadius == 0)
            {
                targetEnemy.GetComponent<Enemy>().TakeDamage(damage);
                return;
            }

            var enemies = GetAoeEnemies(_transform);
            var direction = (targetEnemy.position - transform.position).normalized;
            _animator.transform.forward = direction;
            _animator.SetTrigger("Melee");

            AoeHit.Invoke(enemies, damage, _weaponType);
        }

        private void HandleRangedStandardAttack(int damage, Transform targetEnemy)
        {
            // Instantiate Projectile
            var projectile = Instantiate(_projectilePrefab);
            
            projectile.GetComponent<SpriteRenderer>().sprite = _weapon.Sprite;
            
            projectile.transform
                .DOMove(targetEnemy.position, 0.1f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    // Single target hit
                    if (_aoeRadius == 0)
                    {
                        targetEnemy.GetComponent<Enemy>().TakeDamage(damage);
                        return;
                    }

                    // AOE hit
                    var enemies = GetAoeEnemies(targetEnemy);

                    // Trigger animation in target enemy animator
                    targetEnemy.GetComponentInChildren<Animator>().SetTrigger(_weaponType.ToString());

                    // Enemy Damage
                    AoeHit.Invoke(enemies, damage, _weaponType);
                });
        }
    }
}