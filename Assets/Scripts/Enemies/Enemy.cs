using System;
using Marvin.PoolingSystem;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IPoolingList
{
    public static event Action enemyHasDied;
    private Vector3 targetPosition;
    private int currentWayPoint;
    private int maxWayPoints;
    private LevelManager levelManager;

    [SerializeField] private float movementSpeed;
    [SerializeField] private int baseHealth;
    [SerializeField] private ArmorType armorType;

    private float wayPointCheckInterval = 0.1f;
    private float remainingDistanceToWayPoint = 0.1f;
    private float checkTimer;

    [Space]
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;


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
        levelManager = LevelManager.Instance;
        maxWayPoints = levelManager.enemyWayPoints.Length;
    }
    private void OnEnable()
    {
        currentWayPoint = 1;
        WayPointUpdate();
    }
    private void Update()
    {
        checkTimer += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if (checkTimer >= wayPointCheckInterval)
        {
            checkTimer = 0;

            if (Vector3.Distance(transform.position, targetPosition) < remainingDistanceToWayPoint)
            {
                WayPointUpdate();
            }
        }
    }

    private void WayPointUpdate()
    {
        if (currentWayPoint < maxWayPoints)
        {
            targetPosition = levelManager.GetWayPoint(currentWayPoint);
            currentWayPoint++;
        }
        else
        {
            OnDeath();
        }
    }
    public void SetMaxHealth(float scaling)
    {
        if (scaling <= 0) scaling = 1;

        MaxValue = Mathf.RoundToInt(baseHealth * scaling);
        Value = MaxValue;
    }
    public void TakeDamage(int amount)
    {
        if (amount == 0) return;

        Value -= amount;

        Onhit();

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }
    private void Onhit()
    {
        //HitEffect
    }
    private void OnDeath()
    {
        enemyHasDied?.Invoke();
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
