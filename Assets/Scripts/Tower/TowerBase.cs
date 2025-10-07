using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marvin.PoolingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Tower
{
    public class TowerBase : MonoBehaviour
    {
        public static UnityEvent<TowerBase, BodyObject> BodyPartEquipped { get; } = new();
        public static UnityEvent<TowerBase, BodyObject> BodyPartUnequipped { get; } = new();
        public static UnityEvent<List<Enemy>, int, WeaponType> AoeHit { get; } = new();


        [SerializeField] private TowerValues towerValues;

        [SerializeField] private GameObject _rubyFirePrefab;

        [Space]
        [SerializeField] private LayerMask attackLayer;
        [SerializeField] private float _aoeRadius;
        [SerializeField] private float _currentDamage;
        [SerializeField] private float _currentAttackSpeed;
        private GameObject _projectilePrefab;
        public float _currentRange;
        private float attackSpeedCap = 0.1f;

        private Transform _transform;
        [SerializeField] private WeaponType _weaponType = WeaponType.None;
        private float _timer;
        private float attackTimer;
        private Animator _animator;
        private Transform currentTarget;
        public bool isRecipeTower { get; private set; }

        [Space]
        public BodyObject accessoires;
        public BodyObject head;
        public BodyObject arms;
        public BodyObject body;
        public BodyObject weapon;

        public bool accessoiresSlotUnlocked;
        public bool headSlotUnlocked;
        public bool armsSlotUnlocked;
        public bool bodySlotUnlocked;

        public List<BodyObject> EquippedBodyObjects = new();

        public int recipeMatchPercent { get; private set; } = 0;

        private void Awake()
        {
            _currentDamage = towerValues.baseDamage;
            _currentAttackSpeed = towerValues.baseAttackSpeed;
            CapAttackSpeed();
            _currentRange = towerValues.baseAttackRange;
            _projectilePrefab = towerValues.projectilePrefab;
            _transform = transform;
            _animator = GetComponentInChildren<Animator>();

            if (weapon == null) return;

            _weaponType = weapon.Weapon;
            _aoeRadius = weapon.BonusAoeRadius;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (currentTarget != null) if(currentTarget.gameObject.activeSelf == false) currentTarget = null;

            if (_timer < attackTimer) return;

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
                    _projectilePrefab = bodyObject.ProjectilePrefab;
                    AddTowerValues(bodyObject);
                    break;
            }
        }
        private void AddTowerValues(BodyObject bodyObject)
        {
            // Equip Object and adjust stats

            _currentAttackSpeed -= bodyObject.BonusAttackSpeed;
            _currentDamage += bodyObject.BonusDamage;
            _currentRange += bodyObject.BonusRange;
            _aoeRadius += bodyObject.BonusAoeRadius;

            CapAttackSpeed();

            EquippedBodyObjects.Add(bodyObject);
        }

        private void OnBodyPartUnequipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyObjects.Remove(bodyObject);
            _currentAttackSpeed += bodyObject.BonusAttackSpeed;
            _currentDamage -= bodyObject.BonusDamage;
            _currentRange -= bodyObject.BonusRange;

            _aoeRadius -= bodyObject.BonusAoeRadius;

            EquippedBodyObjects.Remove(bodyObject);
        }
        public void CheckForRecipe()
        {
            if (isRecipeTower) return;
            // Find best recipe match
            int bestMatchPercent = 0;
            foreach (TowerRecipe recipe in Player.Instance.towerRecipes)
            {
                int count = EquippedBodyObjects.Count(bo => recipe.Recipe.Contains(bo));
                int matchPercent = Mathf.RoundToInt((count / (float)recipe.Recipe.Count) * 100);

                Debug.Log(count);
                if (count == recipe.Recipe.Count)
                {
                    TowerBase newTower = Instantiate(recipe.recipeTowerPrefab, transform.position, Quaternion.identity).GetComponentInChildren<TowerBase>();
                    newTower.isRecipeTower = true;
                    IngameController.Instance.playerUI.inventory.SetCurrentTower(newTower);
                    Destroy(gameObject);
                }

                if (matchPercent > bestMatchPercent)
                {
                    bestMatchPercent = matchPercent;
                }
            }
            recipeMatchPercent = bestMatchPercent;
        }
        private void CapAttackSpeed()
        {
            attackTimer = _currentAttackSpeed;
            if (attackTimer < attackSpeedCap) attackTimer = attackSpeedCap;
        }

        private bool Attack()
        {
            if (currentTarget == null || currentTarget.gameObject.activeSelf == false)
            {
                //Check for near targets
                currentTarget = GetClosestEnemy();
            }
            else
            {
                //Check if current target is out of range, if yes switch target
                if (Vector2.Distance(_transform.position, currentTarget.position) > _currentRange)
                {
                    currentTarget = GetClosestEnemy();
                }
            }

            if (currentTarget == null || currentTarget.gameObject.activeSelf == false) return false;

            if (Vector2.Distance(_transform.position, currentTarget.position) > _currentRange) return false;

            var damage = CalculateDamage();

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (_weaponType)
            {
                case WeaponType.None:
                    HandleRangedStandardAttack(damage, currentTarget);
                    break;
                case WeaponType.Boulder:
                case WeaponType.Bow:
                    HandleRangedStandardAttack(damage, currentTarget);
                    break;
                case WeaponType.Crossbow:
                case WeaponType.IceStaff:
                case WeaponType.Scroll:
                case WeaponType.Stone:
                    HandleRangedStandardAttack(damage, currentTarget);
                    break;
                case WeaponType.Club:
                case WeaponType.Dagger:
                case WeaponType.Sword:
                    HandleRangedStandardAttack(damage, currentTarget);
                    break;
                case WeaponType.CrystalStaff:
                    HandleMeleeStandardAttack(damage, currentTarget);
                    break;
                case WeaponType.RubyStaff:
                    StartCoroutine(HandleRubyStaffAttack(damage, currentTarget));
                    break;
            }

            return true;
        }
        

        private Transform GetClosestEnemy()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, _currentRange, attackLayer);
            
            if(cols.Length == 0) return null;
            
            Transform closestEnemy = null;
            var minDistance = float.MaxValue;

            foreach (Collider2D col in cols)
            {
                if (col.TryGetComponent(out Enemy enemy))
                {
                    var distance = Vector2.Distance(_transform.position, enemy.transform.position);

                    if (!(distance < minDistance)) continue;

                    minDistance = distance;
                    closestEnemy = enemy.transform;
                }
                else continue;
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
            return (int)_currentDamage;
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
            Projectile projectile = PoolingSystem.SpawnObject
                (_projectilePrefab, _transform.position, Quaternion.identity, PoolingSystem.PoolingParentGameObject.Projectile).GetComponent<Projectile>();

            projectile.SetValues(targetEnemy, damage, _aoeRadius);
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


//var direction = (targetEnemy.position - transform.position).normalized;
//var angle = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

//projectile.transform.rotation = Quaternion.Euler(angle);


//projectile.transform
//    .DOMove(targetEnemy.position, 0.15f)
//    .SetEase(Ease.OutExpo)
//    .OnComplete(() =>
//    {
//        // Single target hit
//        if (_aoeRadius == 0)
//        {
//            targetEnemy.GetComponentInParent<Enemy>().TakeDamage(damage);
//            Destroy(projectile);
//            return;
//        }

//        var enemies = GetAoeEnemies(targetEnemy);
//        //targetEnemy.GetComponentInChildren<Animator>().SetTrigger(_weaponType.ToString());
//        AoeHit.Invoke(enemies, damage, _weaponType);

//        Destroy(projectile);
//    });