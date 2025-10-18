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
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private TowerValues towerValues;

        [Space]
        [SerializeField] private LayerMask attackLayer;
        private int damageScaling;
        private int bonusDamage;
        [field : SerializeField] public int finalDamage { get; private set; }

        private float baseAttackSpeed;
        private float bonusAttackSpeed;
        [field: SerializeField] public float finalAttackSpeed { get; private set; }

        private int rangeScaling;
        private float bonusRange;
        [field: SerializeField] public float finalRange { get; private set; }

        [field: SerializeField] public float currentAoeRadius { get; private set; }
        private int slowPercentage;
        private float slowDuration;
        private int additionalProjectiles;
        private GameObject projectilePrefab;
        private GameObject objectToSpawn;

        [SerializeField] private bool canDoDoubleDamage;
        private float doubleDamageChance;

        [SerializeField] private bool lifesteal;

        //Targeting
        [SerializeField] private TargetType targetType = TargetType.FollowTarget;
        private float timer;
        private float checkForEnemiesTimer;
        private float checkForEnemiesInterval = 0.05f;
        private Transform currentTarget;

        //Recipe
        public bool isRecipeTower { get; private set; }
        public TowerRecipe currentRecipe { get; private set; }
        public int recipeMatchPercent { get; private set; } = 0;

        //Animation
        private Animator animator;

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


        private void Awake()
        {
            //SetValues
            damageScaling = towerValues.damageScalingPercantage;
            bonusDamage = towerValues.startBonusAttack;
            baseAttackSpeed = towerValues.baseAttackSpeed;
            rangeScaling = towerValues.rangeScalingPercantage;
            bonusRange = towerValues.startBonusRange;
            currentAoeRadius = towerValues.aoeRadius;
            projectilePrefab = towerValues.projectilePrefab;
            targetType = towerValues.targetType;
            slowPercentage = towerValues.slowPercentage;
            slowDuration = towerValues.slowDuration;
            additionalProjectiles = towerValues.additionalProjectiles;
            objectToSpawn = towerValues.objectToSpawn;
            canDoDoubleDamage = towerValues.chanceForDoubleDamage;
            lifesteal = towerValues.lifeSteal;

            finalDamage = CalculateDamage();
            finalAttackSpeed = CalculateAttackSpeed();
            finalRange = CalculateRange();

            animator = GetComponentInChildren<Animator>();
        }
        private void Start()
        {
            if (Player.Instance != null) doubleDamageChance = Player.Instance.GetDoubleDamageChance();
            else doubleDamageChance = 25;

            SortObjects.activeEnemiesSprites.Add(spriteRenderer);
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
                        slowPercentage -= currentAccessoires.SlowPercentage;
                        slowDuration -= currentAccessoires.SlowDuration;
                        additionalProjectiles -= currentAccessoires.AdditionalProjectiles;

                        OnBodyPartUnequipped(tower, currentAccessoires); 
                    }

                    currentAccessoires = bodyObject;
                    slowPercentage += bodyObject.SlowPercentage;
                    slowDuration += bodyObject.SlowDuration;
                    additionalProjectiles += bodyObject.AdditionalProjectiles;

                    if(towerValues.chanceForDoubleDamage == false)
                    {
                        if (currentWeapon != null)
                        {
                            //check if the weapon has crit
                            if (currentWeapon.ChanceForDoubleDamage == false) canDoDoubleDamage = bodyObject.ChanceForDoubleDamage;
                        }
                        else canDoDoubleDamage = bodyObject.ChanceForDoubleDamage;
                    }

                    if (towerValues.lifeSteal == false)
                    {
                        if (currentWeapon != null)
                        {
                            //check if the weapon has lifesteal
                            if (currentWeapon.LifeSteal == false) lifesteal = bodyObject.LifeSteal;
                        }
                        else lifesteal = bodyObject.LifeSteal;
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
                        slowPercentage -= currentWeapon.SlowPercentage;
                        slowDuration -= currentWeapon.SlowDuration;
                        additionalProjectiles -= currentWeapon.AdditionalProjectiles;
                        OnBodyPartUnequipped(tower, currentWeapon); 
                    }

                    currentWeapon = bodyObject;

                    targetType = bodyObject.TargetType;
                    projectilePrefab = bodyObject.ProjectilePrefab;
                    slowPercentage += bodyObject.SlowPercentage;
                    slowDuration += bodyObject.SlowDuration;
                    additionalProjectiles += bodyObject.AdditionalProjectiles;
                    objectToSpawn = bodyObject.ObjectToSpawn;
                    canDoDoubleDamage = bodyObject.ChanceForDoubleDamage;
                    lifesteal = bodyObject.LifeSteal;

                    damageScaling = bodyObject.DamageScalingPercentage;
                    baseAttackSpeed = bodyObject.BaseAttackSpeed;
                    rangeScaling = bodyObject.RangeScalingPercentage;
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
            currentAoeRadius += bodyObject.BonusAoeRadius;

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

            currentAoeRadius -= bodyObject.BonusAoeRadius;

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

                if (matchPercent > bestMatchPercent)
                {
                    bestMatchPercent = matchPercent;
                }


                if (count == recipe.Recipe.Count)
                {
                    currentRecipe = recipe;
                    PlayerPrefs.SetInt(recipe.towerName, 1);
                    UpdateRecipesUI?.Invoke();
                    break;
                }
                else
                {
                    currentRecipe = null;
                }
            }
            recipeMatchPercent = bestMatchPercent;
        }
        public void UpgradeTower()
        {
            if(currentRecipe == null)
            {
                Debug.Log("no recipe found");
                return;
            }

            TowerBase newTower = Instantiate(currentRecipe.recipeTowerPrefab, transform.position, Quaternion.identity).GetComponentInChildren<TowerBase>();
            newTower.isRecipeTower = true;

            IngameController.Instance.playerUI.inventory.SetCurrentTower(newTower);
            if (SortObjects.activeEnemiesSprites.Contains(spriteRenderer))
            {
                Debug.Log("tower in list");
                SortObjects.activeEnemiesSprites.Remove(spriteRenderer);
            }
            Destroy(transform.parent.gameObject);
        }
        private int CalculateDamage()
        {
            return Mathf.RoundToInt(bonusDamage * (damageScaling * 0.01f));
        }
        private float CalculateAttackSpeed()
        {
            return (float)System.Math.Round(baseAttackSpeed / (bonusAttackSpeed * 0.01f + 1), 2);
        }
        private float CalculateRange()
        {
            return (float)System.Math.Round(rangeScaling * (bonusRange * 0.01f),1); 
        }

        private bool Attack()
        {
            //because of pooling
            if (currentTarget != null) if (currentTarget.gameObject.activeSelf == false) currentTarget = null;

            switch (targetType)
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
                if (Vector2.Distance(transform.position, currentTarget.position) > finalRange) currentTarget = GetClosestEnemy();
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
                    float distance = (transform.position - enemy.transform.position).sqrMagnitude;   //Vector2.Distance(_transform.position, enemy.transform.position);

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

            if (additionalProjectiles > 0)
            {
                GameObject[] enemies = Physics2D.OverlapCircleAll(transform.position, finalRange , attackLayer)
                    .OrderBy(x => (x.transform.position - transform.position).sqrMagnitude)  //Vector2.Distance(x.transform.position, transform.position))
                    .Select(x => x.gameObject).ToArray();

                int projectilesFired = 0;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (projectilesFired >= additionalProjectiles) break;

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
                (projectilePrefab, transform.position, Quaternion.identity, PoolingSystem.PoolingParentGameObject.Projectile).GetComponent<Projectile>();

            projectile.damage = finalDamage;
            if (canDoDoubleDamage)
            {
                if(UnityEngine.Random.Range(0 , 100) < doubleDamageChance)
                {
                    projectile.damage = finalDamage * 2;
                }
            }
            projectile.aoeRadius = currentAoeRadius;
            projectile.range = finalRange;
            projectile.slowPercentage = slowPercentage;
            projectile.slowDuration = slowDuration;
            projectile.targetType = targetType;
            projectile.objectToSpawn = objectToSpawn;
            projectile.lifeSteal = lifesteal;

            projectile.SetValues(targetEnemy);
        }
        public float GetTowerRange()
        {
            return finalRange;
        }
        public int GetDamageScaling()
        {
            return damageScaling;
        }
        public int GetRangeScaling()
        {
            return rangeScaling;
        }
        public int GetSlow()
        {
            return slowPercentage;
        }
        public bool GetLifesteal()
        {
            return lifesteal;
        }
        public bool GetDoubleDamage()
        {
            return canDoDoubleDamage;
        }
        public int GetAdditionalProjectiles()
        {
            return additionalProjectiles;
        }
    }
}