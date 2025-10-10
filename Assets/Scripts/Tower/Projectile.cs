using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marvin.PoolingSystem;
using Tower;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolingList
{
    private Transform currenttarget;
    private Transform bulletTarget;
    private Vector3 direction;
    private bool targetHasDied;
    private bool dealedDamage;
    private Vector3 enemyPositionOnProjectileLaunch;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Vector3 objectToSpawnRotation;
    private Vector3 endPosition;
    private float timer;

    [HideInInspector] public int damage;
    [HideInInspector] public float range;
    [HideInInspector] public float aoeRadius;
    [HideInInspector] public int slowPercentage;
    [HideInInspector] public float slowDuration;
    [HideInInspector] public TargetType targetType;
    [HideInInspector] public GameObject objectToSpawn;
    [HideInInspector] public bool lifeSteal;


    [Header("BasicValues")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float disableTimeAfterHit = 0.05f;

    [Space]
    [SerializeField] private int sortingLayerOnEnable = 20;
    [SerializeField] private int sortingLayerAfterHit = -99;        //Used for AimOnGroundOnly

    //Swing
    [Header("Melee")]
    [SerializeField] private float meleeWeaponOffset = 0.5f;
    [SerializeField] private float meleeWeaponScale = 1;
    private float startRotation;
    private float endRotation;
    private float lerpPercentage;

    [Header("ThrowValues")]
    [SerializeField] private GameObject aoeVisualBullet;
    private float startHeight;
    [SerializeField] private float maxHeight;
    private float bulletLifeTime;

    [Header("Pierce")]
    [SerializeField] private float shrinkEachHit = 0.2f;
    [SerializeField] private float damageReductionEachHit = 0.1f;
    private float shrinkMinSize = 0.3f;
    private int targetsHit;
    private List<Collider2D> pierceList;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    void Update()
    {
        switch (targetType)
        {
            case TargetType.FollowTarget:
                ProjectileFollowTarget();
                break;
            case TargetType.AimOnGround:
                AimOnCurrentTargetPosition();
                break;
            case TargetType.Swing:
                Swing();
                break;
            case TargetType.Throw:
                ThrowMovement();
                break;
            case TargetType.Melee:
                break;
            case TargetType.Pierce:
                PierceMovement();
                break;
        }
    }
    public void SetValues(Transform target)
    {
        dealedDamage = false;
        CancelInvoke();
        StopAllCoroutines();
        spriteRenderer.sortingOrder = sortingLayerOnEnable;
        timer = 0;
        lerpPercentage = 0;

        currenttarget = target;
        bulletTarget = currenttarget.GetComponent<Enemy>().GetBulletTarget();
        direction = (bulletTarget.transform.position - transform.position).normalized;
        enemyPositionOnProjectileLaunch = bulletTarget.transform.position;

        if (objectToSpawn != null) objectToSpawnRotation = bulletTarget.position - transform.position;


        switch (targetType)
        {
            case TargetType.AimOnGround:
                transform.right = enemyPositionOnProjectileLaunch - transform.position;
                break;
            case TargetType.Swing:
                SetSwing();
                break;
            case TargetType.Throw:
                SetThrow();
                break;
            case TargetType.Melee:
                SetMelee();
                break;
            case TargetType.Pierce:
                if (pierceList == null) pierceList = new List<Collider2D>();
                pierceList.Clear();

                targetsHit = 0;
                transform.localScale = new Vector3(aoeRadius, aoeRadius, 1);
                endPosition = transform.position + direction * range;      //final position in enemy direction
                transform.right = enemyPositionOnProjectileLaunch - transform.position;
                break;
        }
    }
    private void SetSwing()
    {
        transform.localScale = new Vector3(meleeWeaponScale * range, meleeWeaponScale * range, 1);

        direction = (bulletTarget.transform.position - transform.position).normalized;
        transform.position = transform.position + (direction * meleeWeaponOffset);

        Vector3 targ = bulletTarget.position;
        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg - 55;     //45 = Png angle
        startRotation = angle + (20 * aoeRadius);
        endRotation = angle - (20 * aoeRadius);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, startRotation));

        StartCoroutine(SwingDamageDelay());
    }
    private void SetThrow()
    {
        direction = ((Vector2)enemyPositionOnProjectileLaunch - (Vector2)transform.position).normalized;

        float distance = Vector2.Distance(enemyPositionOnProjectileLaunch, transform.position);

        startHeight = transform.position.z;
        float currentSpeed = projectileSpeed;
        bulletLifeTime = distance / currentSpeed;
        timer = bulletLifeTime;
    }
    private void SetMelee()
    {
        transform.localScale = new Vector3(meleeWeaponScale * range, meleeWeaponScale * range, 1);

        direction = (bulletTarget.transform.position - transform.position).normalized;
        transform.position = transform.position + (direction * meleeWeaponOffset);
        transform.right = enemyPositionOnProjectileLaunch - transform.position;

        DealAoeDamage(bulletTarget.transform.position);

        Invoke("DisableProjectile", disableTimeAfterHit);
    }
    private void ProjectileFollowTarget()
    {
        if (currenttarget != null)
        {
            if (currenttarget.gameObject.activeSelf == true)
            {
                direction = (bulletTarget.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, bulletTarget.position, projectileSpeed * Time.deltaTime);
                transform.right = bulletTarget.position - transform.position;
            }
            else BulletNoTarget();
        }
        else BulletNoTarget();

    }
    private void AimOnCurrentTargetPosition()
    {
        if (dealedDamage) return;

        transform.position = Vector3.MoveTowards(transform.position, enemyPositionOnProjectileLaunch, projectileSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, enemyPositionOnProjectileLaunch) < 0.3f)
        {
            DealAoeDamage(enemyPositionOnProjectileLaunch);
            dealedDamage = true;
            spriteRenderer.sortingOrder = sortingLayerAfterHit;
            Invoke("DisableProjectile", disableTimeAfterHit);
        }
    }
    private void Swing()
    {
        timer += Time.deltaTime;
        lerpPercentage = timer / projectileSpeed;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(startRotation, endRotation, lerpPercentage)));

        if (timer > projectileSpeed)
        {
            DisableProjectile();
        }
    }
    private void ThrowMovement()
    {
        if (dealedDamage) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            dealedDamage = true;

            DealAoeDamage(transform.position);

            spriteRenderer.sortingOrder = sortingLayerAfterHit;
            Invoke("DisableProjectile", disableTimeAfterHit);
            if (aoeVisualBullet != null)
            {
                aoeVisualBullet.transform.position = transform.position;
            }
            return;
        }
        transform.Translate(direction * projectileSpeed * Time.deltaTime, Space.World);

        if (aoeVisualBullet != null)
        {
            if (maxHeight > 0)
            {
                Vector3 deltaHeight = Vector3.zero * startHeight * (1 - timer / bulletLifeTime) + new Vector3(0, -maxHeight * Mathf.Sin((timer / bulletLifeTime) * Mathf.PI), 0);
                aoeVisualBullet.transform.position = transform.position - deltaHeight;
            }
        }
    }
    private void PierceMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPosition, projectileSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, endPosition) < 0.3f)
        {
            Invoke("DisableProjectile", disableTimeAfterHit);
        }
    }
    private void BulletNoTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, projectileSpeed * Time.deltaTime);
        if (targetHasDied == false)
        {
            targetHasDied = true;
            Invoke("DisableProjectile", 0.3f);
        }
    }
    private void FollowTargetDamage()
    {
        if (aoeRadius <= 0)
        {
            if (objectToSpawn)
            {
                SpawnObject();
                return;
            }

            if (currenttarget.TryGetComponent(out Enemy enemy))
            {
                if (enemy.gameObject.activeSelf == false) return;

                enemy.TakeDamage(damage, lifeSteal);
                if (slowPercentage > 0 && enemy.gameObject.activeSelf == true) enemy.DoSlow(slowPercentage, slowDuration);
            }
        }
        else
        {
            DealAoeDamage(bulletTarget.position);  //* 0.01f
        }
    }
    private void DealAoeDamage(Vector3 position)
    {
        if (objectToSpawn)
        {
            SpawnObject();
            return;
        }

        Collider2D[] colls = Physics2D.OverlapCircleAll(position, aoeRadius, hitLayer);

        foreach (Collider2D coll in colls)
        {
            if (coll.gameObject.activeSelf == false) continue;

            if (coll.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage, lifeSteal);

                if (slowPercentage > 0 && enemy.gameObject.activeSelf == true) enemy.DoSlow(slowPercentage, slowDuration);
            }
        }
    }
    IEnumerator SwingDamageDelay()
    {
        yield return new WaitForSeconds(projectileSpeed * 0.2f);
        DealAoeDamage(currenttarget.position);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dealedDamage == true) return;

        switch (targetType)
        {
            case TargetType.FollowTarget:
                if (collision.gameObject == currenttarget.gameObject)
                {
                    dealedDamage = true;
                    FollowTargetDamage();
                    Invoke("DisableProjectile", disableTimeAfterHit);
                }
                break;
            case TargetType.Pierce:
                if (collision.gameObject.TryGetComponent(out Enemy enemy))
                {
                    if (pierceList.Contains(collision) == false)
                    {
                        pierceList.Add(collision);
                        int finalDamage = Mathf.RoundToInt(damage * (1 - damageReductionEachHit * targetsHit));
                        if (finalDamage < 1) finalDamage = 1;
                        enemy.TakeDamage(finalDamage, lifeSteal);

                        targetsHit++;

                        float newScale = transform.localScale.x - shrinkEachHit;
                        if (newScale <= shrinkMinSize)
                        {
                            dealedDamage = true;
                            Invoke("DisableProjectile", disableTimeAfterHit);
                        }
                        else
                        {
                            transform.localScale = new Vector3(newScale, newScale, 1);
                        }
                    }
                }
                break;
        }
    }
    private void SpawnObject()
    {
        GameObject spawnedObject = PoolingSystem.SpawnObject
             (objectToSpawn, transform.position, Quaternion.identity, PoolingSystem.PoolingParentGameObject.Projectile);

        if (spawnedObject.TryGetComponent(out DealDmgOnEnter dealDmgOnEnter))
        {
            if (!dealDmgOnEnter.ResetRotation)
            {
                spawnedObject.transform.right = objectToSpawnRotation;
            }

            dealDmgOnEnter.damage = damage;
            if (dealDmgOnEnter.baseScalingSaved == false)
            {
                dealDmgOnEnter.baseScalingSaved = true;
                dealDmgOnEnter.baseScaling = objectToSpawn.transform.localScale;
            }

            float xScale = objectToSpawn.transform.localScale.x * (aoeRadius + 1);
            if (xScale < 0.1f) xScale = 0.1f;
            float yScale = objectToSpawn.transform.localScale.y * (aoeRadius + 1);
            if (yScale < 0.1f) yScale = 0.1f;
            dealDmgOnEnter.transform.localScale = new Vector3(xScale, yScale, 1);
        }
    }
    private void DisableProjectile()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
