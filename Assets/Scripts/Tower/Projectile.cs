using System.Collections;
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
    private float timer;

    [HideInInspector] public int damage;
    [HideInInspector] public float range;
    [HideInInspector] public float aoeRadius;
    [HideInInspector] public bool slow;
    [HideInInspector] public float slowPercentage;
    [HideInInspector] public float slowDuration;
    [HideInInspector] public TargetType targetType;

    //Swing
    private float startRotation;
    private float endRotation;
    private float lerpPercentage;

    [Header("BasicValues")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float disableTimeAfterHit = 0.05f;

    [Space]
    [SerializeField] private int sortingLayerOnEnable = 20;
    [SerializeField] private int sortingLayerAfterHit = -99;        //Used for AimOnGroundOnly

    [Header("ThrowValues")]
    [SerializeField] private GameObject aoeVisualBullet;
    private float startHeight;
    [SerializeField] private float maxHeight;
    private float bulletLifeTime;


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
        enemyPositionOnProjectileLaunch = bulletTarget.transform.position;

        switch (targetType)
        {
            case TargetType.AimOnGround:
                transform.right = enemyPositionOnProjectileLaunch - transform.position;
                break;
            case TargetType.Swing:
                transform.localScale = new Vector3(0.5f * range, 0.5f * range, 1);

                Vector3 targ = bulletTarget.position;
                Vector3 objectPos = transform.position;
                targ.x = targ.x - objectPos.x;
                targ.y = targ.y - objectPos.y;

                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg - 45;     //45 = Png angle
                startRotation = angle + (20 * aoeRadius);
                endRotation = angle - (20 * aoeRadius);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, startRotation));

                StartCoroutine(SwingDelay());
                break;
            case TargetType.Throw:
                direction = ((Vector2)enemyPositionOnProjectileLaunch - (Vector2)transform.position).normalized;

                float distance = Vector2.Distance(enemyPositionOnProjectileLaunch, transform.position);

                startHeight = transform.position.z;
                float currentSpeed = projectileSpeed;
                bulletLifeTime = distance / currentSpeed;
                timer = bulletLifeTime;
                break;
        }
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

        direction = (enemyPositionOnProjectileLaunch - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, enemyPositionOnProjectileLaunch, projectileSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, enemyPositionOnProjectileLaunch) < 0.3f)
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
                Vector3 deltaHeight = Vector3.zero * startHeight * (1 - timer / bulletLifeTime) + new Vector3(0,-maxHeight * Mathf.Sin((timer / bulletLifeTime) * Mathf.PI), 0);
                aoeVisualBullet.transform.position = transform.position - deltaHeight;
            }
        }
    }
    private void BulletNoTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, projectileSpeed * Time.deltaTime);
        if (targetHasDied == false)
        {
            targetHasDied = true;
            Invoke("DisableProjectile", 0.5f);
        }
    }
    private void FollowTargetDamage()
    {
        if (aoeRadius <= 0)
        {
            if (currenttarget.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                if (slow == true) enemy.DoSlow(slowPercentage, slowDuration);
            }          
        }
        else
        {
            DealAoeDamage(bulletTarget.position);  //* 0.01f
        }
    }
    private void DealAoeDamage(Vector3 position)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(position, aoeRadius, hitLayer);

        foreach (Collider2D coll in colls)
        {
            if (coll.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);

                if (slow == true && enemy.gameObject.activeSelf == true) enemy.DoSlow(slowPercentage, slowDuration);
            }
        }
    }
    IEnumerator SwingDelay()
    {
        yield return new WaitForSeconds(projectileSpeed * 0.5f);
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
                    FollowTargetDamage();
                    dealedDamage = true;
                    Invoke("DisableProjectile", disableTimeAfterHit);
                }
                break;
        }
    }
    private void DisableProjectile()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
