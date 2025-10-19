using System;
using System.Collections.Generic;
using Marvin.PoolingSystem;
using UnityEngine;

public class LootGoober : MonoBehaviour, IPoolingList
{
    private Vector3 targetPosition;
    private int currentWayPoint;
    private int maxWayPoints;
    private LevelManager levelManager;
    private bool faceRight;
    private SpriteRenderer spriteRenderer;

    public float CollisionCheckInterval = 0.2f;
    public float GobbleDistance = 1.0f;
    public List<Transform> LootGooberWayPoints;
    private float _lastCheckTime;
    private Dictionary<WorldItem, Transform> _gobbledLoots = new();

    [SerializeField] private float baseMovementSpeed = 4;
    private float currentMovementSpeed;

    private float wayPointCheckInterval = 0.1f;
    private float remainingDistanceToWayPoint = 0.1f;
    private float checkTimer;


    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (LevelManager.Instance != null)
        {
            levelManager = LevelManager.Instance;
            maxWayPoints = levelManager.LootGooberWayPoints.Count;
        }
    }
    private void OnEnable()
    {
        StopAllCoroutines();

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
        if (currentWayPoint == maxWayPoints + 1)
        {
            Despawn();
            return;
        }

        if (currentWayPoint < maxWayPoints)
        {
            targetPosition = levelManager.GetGooberWayPoint(currentWayPoint);
            CheckRotation();
            currentWayPoint++;
        }
        else
        {
            currentWayPoint++;
            var randomUnitCircle = UnityEngine.Random.insideUnitCircle.normalized * 25f;
            targetPosition = transform.position + (Vector3)randomUnitCircle;

            if (transform.position.x < targetPosition.x && faceRight == true) flip();
            if (transform.position.x > targetPosition.x && faceRight == false) flip();
        }
    }
    public void CheckRotation()
    {
        if (transform.position.x < levelManager.GetGooberWayPoint(currentWayPoint).x && faceRight == true) flip();
        if (transform.position.x > levelManager.GetGooberWayPoint(currentWayPoint).x && faceRight == false) flip();
    }
    private void flip()
    {
        faceRight = !faceRight;
        Vector3 localScale;
        localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    private void Despawn()
    {
        StopAllCoroutines();

        SortObjects.activeEnemiesSprites.Remove(spriteRenderer);

        if (LevelManager.Instance != null)
        {
            foreach (var loot in _gobbledLoots)
            {
                loot.Key.transform.SetParent(null);
                loot.Key.ReturnToItemPool();
                Destroy(loot.Key.gameObject);
            }
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
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
