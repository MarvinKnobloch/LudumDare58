using System;
using System.Collections;
using System.Collections.Generic;
using Marvin.PoolingSystem;
using UnityEngine;

public class LootGoober : MonoBehaviour, IPoolingList
{
    public static event Action enemyHasDied;
    private Vector3 targetPosition;
    private int currentWayPoint;
    private int maxWayPoints;
    private LevelManager levelManager;
    private bool faceRight;
    private SpriteRenderer spriteRenderer;

    public float CollisionCheckInterval = 0.2f;
    public float GobbleDistance = 1.0f;
    private float _lastCheckTime;
    private Dictionary<WorldItem, Transform> _gobbledLoots = new();


    [SerializeField] private float baseMovementSpeed = 4;
    private float currentMovementSpeed;
    [SerializeField] private int baseHealth;
    [SerializeField] private ArmorType armorType;

    private float wayPointCheckInterval = 0.1f;
    private float remainingDistanceToWayPoint = 0.1f;
    private float checkTimer;

    [Space]
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;

    //Slow
    private float slowPercentage;
    private float slowDuration;
    private Coroutine slowCoroutine;
    [SerializeField] private Color slowColor;

    [Header("Other")]
    [SerializeField] private int damageToPlayer;
    [SerializeField] private int soulsDropAmount;
    [SerializeField] private Transform bulletTarget;

    [Header("Drops")]
    private EnemyDrop itemDropper;

    [Header("HitEffect")]
    [SerializeField] private int hitEffectAmount = 5;
    private Coroutine blinkEffect;
    private int currentHitEffectAmount;
    [SerializeField] private float hitEffectDuration = 0.1f;

    //animator
    private Animator animator;
    private float normalAnimationSpeed;


    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    public int Value
    {
        get { return currentHealth; }
        set { currentHealth = Math.Min(Math.Max(0, value), maxHealth); }
    }
    public int MaxValue
    {
        get { return maxHealth; }
        set { maxHealth = Math.Max(0, value); currentHealth = Math.Min(value, currentHealth); }
    }
    public enum ArmorType
    {
        Cloth,
        Basic,
        Armored,
        Balance,
        Magic,
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemDropper = GetComponent<EnemyDrop>();
        animator = GetComponent<Animator>();
        normalAnimationSpeed = animator.speed;

        if (LevelManager.Instance != null)
        {
            levelManager = LevelManager.Instance;
            maxWayPoints = levelManager.enemyWayPoints.Length;
        }
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        blinkEffect = null;
        slowCoroutine = null;

        currentMovementSpeed = baseMovementSpeed;
        spriteRenderer.color = Color.white;

        SortObjects.activeEnemiesSprites.Add(spriteRenderer);

        currentWayPoint = 1;
        if (LevelManager.Instance != null) WayPointUpdate();

    }
    private void Update()
    {
        checkTimer += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMovementSpeed * Time.deltaTime);

        if (checkTimer >= wayPointCheckInterval)
        {
            checkTimer = 0;

            if (Vector3.Distance(transform.position, targetPosition) < remainingDistanceToWayPoint)
            {
                WayPointUpdate();
            }
        }

        if (Time.time - _lastCheckTime >= CollisionCheckInterval)
        {
            _lastCheckTime = Time.time;
            CheckForOverlap();
        }
    }

    private void WayPointUpdate()
    {
        if (currentWayPoint < maxWayPoints)
        {
            targetPosition = levelManager.GetWayPoint(currentWayPoint);
            CheckRotation();
            currentWayPoint++;
        }
        else
        {
            Player.Instance.TakeDamage(damageToPlayer);
            Despawn();
        }
    }
    public void CheckRotation()
    {
        if (transform.position.x < levelManager.GetWayPoint(currentWayPoint).x && faceRight == true) flip();
        if (transform.position.x > levelManager.GetWayPoint(currentWayPoint).x && faceRight == false) flip();
    }
    private void flip()
    {
        faceRight = !faceRight;
        Vector3 localScale;
        localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    public void SetMaxHealth(float scaling)
    {
        if (scaling <= 0) scaling = 1;

        MaxValue = Mathf.RoundToInt(baseHealth * scaling);
        Value = MaxValue;
    }
    public void TakeDamage(int amount, bool lifeSteal)
    {
        if (amount == 0) return;

        Value -= amount;

        IngameController.Instance.floatingNumberController.displaynumber(transform.position, amount, Color.red);

        if (currentHealth > 0)
        {
            BlinkEffect();
        }
        else
        {

            if (itemDropper != null)
            {
                itemDropper.DropItems(transform.position);
            }
            if (lifeSteal == true)
            {
                if (UnityEngine.Random.Range(0, 100) < Player.Instance.GetLifeStealChance())
                {
                    Player.Instance.Heal(1);
                }
            }
            //DesiDONE
            Player.Instance.UpdateSouls(soulsDropAmount);
            Despawn();
        }
    }
    private void BlinkEffect()
    {
        if (blinkEffect != null) return;
        //StopCoroutine("StartBlinkEffect");

        spriteRenderer.color = Color.white;
        currentHitEffectAmount = 0;

        blinkEffect = StartCoroutine("StartBlinkEffect");
    }
    IEnumerator StartBlinkEffect()
    {
        while (currentHitEffectAmount < hitEffectAmount)
        {
            if (currentHitEffectAmount % 2 == 0) spriteRenderer.color = Color.red;
            else
            {
                if (slowCoroutine == null) spriteRenderer.color = Color.white;
                else spriteRenderer.color = slowColor;
            }

            currentHitEffectAmount++;
            yield return new WaitForSeconds(hitEffectDuration);
        }

        if (slowCoroutine == null) spriteRenderer.color = Color.white;
        else spriteRenderer.color = slowColor;
        blinkEffect = null;
    }
    public void DoSlow(float _slowPercentage, float _slowDuration)
    {
        float finalSlow = 1 - _slowPercentage * 0.01f;

        if (currentMovementSpeed >= (baseMovementSpeed * finalSlow))          //if new Slow is worse do nothing
        {
            StopCoroutine("Slow");
            slowCoroutine = null;

            currentMovementSpeed = baseMovementSpeed;
            slowPercentage = finalSlow;
            slowDuration = _slowDuration;

            slowCoroutine = StartCoroutine("Slow");
        }
    }

    IEnumerator Slow()
    {
        animator.speed *= slowPercentage;
        currentMovementSpeed *= slowPercentage;
        if (blinkEffect == null) spriteRenderer.color = slowColor;

        yield return new WaitForSeconds(slowDuration);

        animator.speed = normalAnimationSpeed;
        if (blinkEffect == null) spriteRenderer.color = Color.white;
        currentMovementSpeed = baseMovementSpeed;

        slowCoroutine = null;
    }
    private void Despawn()
    {
        StopAllCoroutines();
        slowCoroutine = null;
        blinkEffect = null;

        //animator.speed = normalAnimationSpeed;

        SortObjects.activeEnemiesSprites.Remove(spriteRenderer);

        if (LevelManager.Instance != null)
        {
            enemyHasDied?.Invoke();
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
    }

    public Transform GetBulletTarget()
    {
        return bulletTarget;
    }
    public void GobbleItem(WorldItem item)
    {
        _gobbledLoots.Add(item, item.transform);
        item.transform.SetParent(transform);
        item.TryGetComponent(out LootPop lootPop);

        if (lootPop != null)
        {
            Destroy(lootPop);
        }
    }
    void CheckForOverlap()
    {
        var worldItems = FindObjectsByType<WorldItem>(FindObjectsSortMode.None);
        var position = transform.position;

        foreach (WorldItem item in worldItems)
        {
            if (_gobbledLoots.ContainsKey(item)) continue;
            if (Vector2.Distance(position, item.transform.position) <= GobbleDistance)
            {
                GobbleItem(item);
            }
        }
    }
}
