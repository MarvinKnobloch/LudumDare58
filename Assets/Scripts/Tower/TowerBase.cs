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

        public static event Action UpdateRecipesUI;

        [SerializeField] private TowerValues towerValues;

        [Space]
        [SerializeField] private LayerMask attackLayer;
        private int damageScaling;
        private int baseDamage;   
        private int bonusDamage;
        [SerializeField] private int finalDamage;

        private float baseAttackSpeed;
        private float bonusAttackSpeed;
        [SerializeField] private float finalAttackSpeed;

        private float baseRange;
        private float bonusRange;
        [SerializeField] private float finalRange;

        [SerializeField] private float _currentAoeRadius;
        private int _slowPercentage;
        private float _slowDuration;
        private int _additionalProjectiles;
        private GameObject _projectilePrefab;
        private GameObject _objectToSpawn;

        [SerializeField] private bool chanceForDoubleDamage;
        private float doubleDamageChance = 25;

        [SerializeField] private bool lifeSteal;

        private Transform _transform;
        [SerializeField] private TargetType _targetType = TargetType.FollowTarget;
        private float timer;
        private float checkForEnemiesTimer;
        private float checkForEnemiesInterval = 0.05f;
        private Animator _animator;
        private Transform currentTarget;
        public bool isRecipeTower { get; private set; }

        [Space]
        public BodyObject currentAccessoires;
        public BodyObject currentHead;
        public BodyObject currentArms;
        public BodyObject currentBody;
        public BodyObject currentWeapon;

        public bool accessoiresSlotUnlocked;
        public bool headSlotUnlocked;
        public bool armsSlotUnlocked;
        public bool bodySlotUnlocked;

        public List<BodyObject> EquippedBodyObjects = new();

        public int recipeMatchPercent { get; private set; } = 0;

        private void Awake()
        {
            //SetValues
            damageScaling = towerValues.damageScaling;
            baseDamage = towerValues.baseDamage;
            baseAttackSpeed = towerValues.baseAttackSpeed;
            baseRange = towerValues.baseAttackRange;
            _currentAoeRadius = towerValues.aoeRadius;
            _projectilePrefab = towerValues.projectilePrefab;
            _targetType = towerValues.targetType;
            _slowPercentage = towerValues.slowPercentage;
            _slowDuration = towerValues.slowDuration;
            _additionalProjectiles = towerValues.additionalProjectiles;
            _objectToSpawn = towerValues.objectToSpawn;
            chanceForDoubleDamage = towerValues.chanceForDoubleDamage;

            finalDamage = CalculateDamage();
            finalAttackSpeed = CalculateAttackSpeed();
            finalRange = CalculateRange();

            _transform = transform;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            checkForEnemiesTimer += Time.deltaTime;

            if (timer > finalAttackSpeed)
            {
                if(checkForEnemiesTimer > checkForEnemiesInterval)
                {
                    checkForEnemiesTimer = 0;
                    if (Attack()) timer = 0;
                }
            }
        }

        public void OnBodyPartEquipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            switch (bodyObject.Part)
            {
                case BodyPart.Accessory:
                    if (currentAccessoires != null)
                    {
                        _slowPercentage -= currentAccessoires.SlowPercentage;
                        _slowDuration -= currentAccessoires.SlowDuration;
                        _additionalProjectiles -= currentAccessoires.AdditionalProjectiles;

                        OnBodyPartUnequipped(tower, currentAccessoires); 
                    }

                    currentAccessoires = bodyObject;
                    _slowPercentage += bodyObject.SlowPercentage;
                    _slowDuration += bodyObject.SlowDuration;
                    _additionalProjectiles += bodyObject.AdditionalProjectiles;
                    lifeSteal = bodyObject.LifeSteal;

                    if(towerValues.chanceForDoubleDamage == false)
                    {
                        if (currentWeapon != null)
                        {
                            //check if the weapon has crit
                            if (currentWeapon.ChanceForDoubleDamage == false) chanceForDoubleDamage = bodyObject.ChanceForDoubleDamage;
                        }
                        else chanceForDoubleDamage = bodyObject.ChanceForDoubleDamage;
                    }

                    break;
                case BodyPart.Head:
                    if (currentHead != null) OnBodyPartUnequipped(tower, currentHead);
                    currentHead = bodyObject;
                    break;
                case BodyPart.Arm:
                    if (currentArms != null) OnBodyPartUnequipped(tower, currentArms);
                    currentArms = bodyObject;
                    break;
                case BodyPart.Torso:
                    if (currentBody != null) OnBodyPartUnequipped(tower, currentBody);
                    currentBody = bodyObject;
                    break;
                case BodyPart.Weapon:
                    if (currentWeapon != null)
                    {
                        _slowPercentage -= currentWeapon.SlowPercentage;
                        _slowDuration -= currentWeapon.SlowDuration;
                        _additionalProjectiles -= currentWeapon.AdditionalProjectiles;
                        OnBodyPartUnequipped(tower, currentWeapon); 
                    }

                    currentWeapon = bodyObject;

                    _targetType = bodyObject.TargetType;
                    _projectilePrefab = bodyObject.ProjectilePrefab;
                    _slowPercentage += bodyObject.SlowPercentage;
                    _slowDuration += bodyObject.SlowDuration;
                    _additionalProjectiles += bodyObject.AdditionalProjectiles;
                    _objectToSpawn = bodyObject.ObjectToSpawn;

                    damageScaling = bodyObject.DamageScaling;
                    baseDamage = bodyObject.BaseDamage;
                    baseAttackSpeed = bodyObject.BaseAttackSpeed;
                    baseRange = bodyObject.BaseRange;
                    break;
            }
            AddTowerValues(bodyObject);
        }
        private void AddTowerValues(BodyObject bodyObject)
        {
            // Equip Object and adjust stats

            bonusAttackSpeed += bodyObject.BonusAttackSpeed;
            bonusDamage += bodyObject.BonusDamage;
            bonusRange += bodyObject.BonusRange;
            _currentAoeRadius += bodyObject.BonusAoeRadius;

            finalDamage = CalculateDamage();
            finalAttackSpeed = CalculateAttackSpeed();
            finalRange = CalculateRange();

            EquippedBodyObjects.Add(bodyObject);
        }

        private void OnBodyPartUnequipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;

            EquippedBodyObjects.Remove(bodyObject);
            bonusAttackSpeed -= bodyObject.BonusAttackSpeed;
            bonusDamage -= bodyObject.BonusDamage;
            bonusRange -= bodyObject.BonusRange;

            _currentAoeRadius -= bodyObject.BonusAoeRadius;

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

                if (count == recipe.Recipe.Count)
                {
                    TowerBase newTower = Instantiate(recipe.recipeTowerPrefab, transform.position, Quaternion.identity).GetComponentInChildren<TowerBase>();
                    newTower.isRecipeTower = true;
                    PlayerPrefs.SetInt(recipe.towerName, 1);
                    UpdateRecipesUI?.Invoke();
                    IngameController.Instance.playerUI.inventory.SetCurrentTower(newTower);
                    Destroy(transform.parent.gameObject);
                }

                if (matchPercent > bestMatchPercent)
                {
                    bestMatchPercent = matchPercent;
                }
            }
            recipeMatchPercent = bestMatchPercent;
        }
        private int CalculateDamage()
        {
            return baseDamage + Mathf.RoundToInt(bonusDamage * (damageScaling * 0.01f));
        }
        private float CalculateAttackSpeed()
        { 
            return baseAttackSpeed / (bonusAttackSpeed * 0.01f + 1);
        }
        private float CalculateRange()
        {
            return baseRange * (bonusRange * 0.01f + 1); 
        }

        private bool Attack()
        {
            //because of pooling
            if (currentTarget != null) if (currentTarget.gameObject.activeSelf == false) currentTarget = null;

            switch (_targetType)
            {
                case TargetType.FollowTarget:
                    StayOnTarget();
                    break;
                case TargetType.AimOnGround:
                    currentTarget = GetClosestEnemy();
                    break;
                case TargetType.Swing:
                    currentTarget = GetClosestEnemy();
                    break;
                case TargetType.Throw:
                    currentTarget = GetClosestEnemy();
                    break;
                case TargetType.Melee:
                    StayOnTarget();
                    break;
                case TargetType.Pierce:
                    currentTarget = GetClosestEnemy();
                    break;

            }

            if (currentTarget == null || currentTarget.gameObject.activeSelf == false) return false;

            HandleRangedStandardAttack(currentTarget);

            return true;
        }
        
        private void StayOnTarget()
        {
            //If no target, get closest Target
            if (currentTarget == null || currentTarget.gameObject.activeSelf == false) currentTarget = GetClosestEnemy();
            else
            {
                //Check if current target is out of range, if yes switch target
                if (Vector2.Distance(_transform.position, currentTarget.position) > finalRange) currentTarget = GetClosestEnemy();
            }
        }
        private Transform GetClosestEnemy()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, finalRange, attackLayer);
            
            if(cols.Length == 0) return null;
            
            Transform closestEnemy = null;
            float minDistance = float.MaxValue;

            foreach (Collider2D col in cols)
            {
                if (col.gameObject.activeSelf == false) continue;

                if (col.TryGetComponent(out Enemy enemy))
                {
                    float distance = (_transform.position - enemy.transform.position).sqrMagnitude;   //Vector2.Distance(_transform.position, enemy.transform.position);

                    if (!(distance < minDistance)) continue;

                    minDistance = distance;
                    closestEnemy = enemy.transform;
                }
                else continue;
            }

            return closestEnemy;
        }
        private void HandleRangedStandardAttack(Transform targetEnemy)
        {
            CreateProjectile(targetEnemy);

            if (_additionalProjectiles > 0)
            {
                GameObject[] enemies = Physics2D.OverlapCircleAll(transform.position, finalRange , attackLayer)
                    .OrderBy(x => (x.transform.position - transform.position).sqrMagnitude)  //Vector2.Distance(x.transform.position, transform.position))
                    .Select(x => x.gameObject).ToArray();

                int projectilesFired = 0;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (projectilesFired >= _additionalProjectiles) break;

                    if (enemies[i].gameObject == targetEnemy.gameObject || enemies[i].gameObject.activeSelf == false) continue;

                    if (enemies[i].TryGetComponent(out Enemy enemy))
                    {
                        CreateProjectile(enemy.transform);
                        projectilesFired++;
                    }
                }
            }
        }
        
        private void CreateProjectile(Transform targetEnemy)
        {
            Projectile projectile = PoolingSystem.SpawnObject
                (_projectilePrefab, _transform.position, Quaternion.identity, PoolingSystem.PoolingParentGameObject.Projectile).GetComponent<Projectile>();

            projectile.damage = finalDamage;
            if (chanceForDoubleDamage)
            {
                if(UnityEngine.Random.Range(0 , 100) < doubleDamageChance)
                {
                    projectile.damage = finalDamage * 2;
                }
            }
            projectile.aoeRadius = _currentAoeRadius;
            projectile.range = finalRange;
            projectile.slowPercentage = _slowPercentage;
            projectile.slowDuration = _slowDuration;
            projectile.targetType = _targetType;
            projectile.objectToSpawn = _objectToSpawn;
            projectile.lifeSteal = lifeSteal;

            projectile.SetValues(targetEnemy);
        }
        public float GetTowerRange()
        {
            return finalRange;
        }
    }
}