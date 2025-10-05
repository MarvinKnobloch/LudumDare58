using System;
using System.Collections;
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

        [SerializeField] private float _baseCooldownModifier = 1;
        [SerializeField] private float _baseDamageModifier = 1;
        [SerializeField] private float _baseRangeModifier = 1;

        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _rubyFirePrefab;

        private float _aoeRadius;
        private float _currentCooldown;
        private float _currentDamage;
        private float _currentRange;
        private Transform _transform;
        private WeaponType _weaponType = WeaponType.None;
        private float _timer;
        private Animator _animator;

        public BodyObject accessoires;
        public BodyObject head;
        public BodyObject arms;
        public BodyObject body;
        public BodyObject weapon;

        public List<BodyObject> EquippedBodyObjects = new();
        public List<TowerRecipe> TowerRecipes = new();
        
        public float RecipeMatchPercent { get; private set; }

        private void Awake()
        {
            _currentRange = _baseRangeModifier;
            _currentDamage = _baseDamageModifier;
            _currentCooldown = _baseCooldownModifier;
            _transform = transform;
            _animator = GetComponentInChildren<Animator>();

            if (weapon == null) return;

            _weaponType = weapon.Weapon;
            _aoeRadius = weapon.AoeRadius;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < _currentCooldown) return;

            if (Attack()) _timer = 0;
        }

        public void OnBodyPartEquipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            switch (bodyObject.Part)
            {
                case BodyPart.Accessory:
                    if (accessoires != null) OnBodyPartUnequipped(tower, accessoires);
                    accessoires = bodyObject;
                    AddTowerValues(bodyObject);
                    break;
                case BodyPart.Head:
                    if (head != null) OnBodyPartUnequipped(tower, head);
                    head = bodyObject;
                    AddTowerValues(bodyObject);
                    break;
                case BodyPart.Arm:
                    if (arms != null) OnBodyPartUnequipped(tower, arms);
                    arms = bodyObject;
                    AddTowerValues(bodyObject);
                    break;
                case BodyPart.Torso:
                    if (body != null) OnBodyPartUnequipped(tower, body);
                    body = bodyObject;
                    AddTowerValues(bodyObject);
                    break;
                case BodyPart.Weapon:
                    if (weapon != null) OnBodyPartUnequipped(tower, weapon);
                    weapon = bodyObject;
                    _weaponType = bodyObject.Weapon;
                    AddTowerValues(bodyObject);
                    break;
            }
        }
        private void AddTowerValues(BodyObject bodyObject)
        {
            // Equip Object and adjust stats

            _currentCooldown *= bodyObject.AttackSpeedModifier > 0 ? bodyObject.AttackSpeedModifier : 1;
            _currentDamage *= bodyObject.DamageModifier > 0 ? bodyObject.DamageModifier : 1;
            _currentRange *= bodyObject.RangeModifier > 0 ? bodyObject.RangeModifier : 1;
            _aoeRadius = bodyObject.AoeRadius;

            EquippedBodyObjects.Add(bodyObject);

            // Find best recipe match
            float bestMatchPercent = 0;
            foreach (var recipe in TowerRecipes)
            {
                var count = EquippedBodyObjects.Count(bo => recipe.Recipe.Contains(bo));
                var matchPercent = count / (float)recipe.Recipe.Count;

                if (count == recipe.Recipe.Count)
                {
                    // ToDo: Recipe Fullfilled - what now?
                }

                if (matchPercent > bestMatchPercent)
                {
                    bestMatchPercent = matchPercent;
                }
            }

            RecipeMatchPercent = bestMatchPercent;
        }

        private void OnBodyPartUnequipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyObjects.Remove(bodyObject);
            _currentCooldown /= bodyObject.AttackSpeedModifier > 0 ? bodyObject.AttackSpeedModifier : 1;
            _currentDamage /= bodyObject.DamageModifier > 0 ? bodyObject.DamageModifier : 1;
            _currentRange /= bodyObject.RangeModifier > 0 ? bodyObject.RangeModifier : 1;

            _aoeRadius = 0;

            EquippedBodyObjects.Remove(bodyObject);
        }

        private bool Attack()
        {
            var enemy = GetClosestEnemy();

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
                case WeaponType.CrystalStaff:
                    HandleMeleeStandardAttack(damage, enemy);
                    break;
                case WeaponType.RubyStaff:
                    StartCoroutine(HandleRubyStaffAttack(damage, enemy));
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
            return Mathf.RoundToInt(weapon.WeaponDamage * _currentDamage);
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
            var angle = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            if (_animator != null)
            {
                _animator.transform.rotation = Quaternion.Euler(angle);
                _animator.SetTrigger("Melee");
            }

            AoeHit.Invoke(enemies, damage, _weaponType);
        }

        private void HandleRangedStandardAttack(int damage, Transform targetEnemy)
        {
            var projectile = Instantiate(_projectilePrefab, _transform.position, Quaternion.identity);

            var direction = (targetEnemy.position - transform.position).normalized;
            var angle = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            projectile.transform.rotation = Quaternion.Euler(angle);
            projectile.GetComponent<SpriteRenderer>().sprite = weapon.ProjectileSprite;

            projectile.transform
                .DOMove(targetEnemy.position, 0.15f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    // Single target hit
                    if (_aoeRadius == 0)
                    {
                        targetEnemy.GetComponent<Enemy>().TakeDamage(damage);
                        Destroy(projectile);
                        return;
                    }

                    var enemies = GetAoeEnemies(targetEnemy);
                    targetEnemy.GetComponentInChildren<Animator>().SetTrigger(_weaponType.ToString());
                    AoeHit.Invoke(enemies, damage, _weaponType);

                    Destroy(projectile);
                });
        }

        private IEnumerator HandleRubyStaffAttack(int damage, Transform targetEnemy)
        {
            var direction = (targetEnemy.position - transform.position).normalized;
            var towerPos = _transform.position;

            for (var i = 0; i < 9; i++)
            {
                var position = new Vector2(towerPos.x + direction.x * i / 1.5f, towerPos.y + direction.y * i / 1.5f);
                var fire = Instantiate(_rubyFirePrefab, position, Quaternion.identity).GetComponent<RubyStaffFire>();
                fire.Initialize(damage);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}