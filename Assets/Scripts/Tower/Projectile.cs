using Marvin.PoolingSystem;
using Tower;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolingList
{
    private Transform currenttarget;
    private Transform bulletTarget;
    private Vector3 direction;
    private bool targetHasDied;
    private bool dealedDamage;
    private TargetType targetType;
    private Vector3 enemyPositionOnProjectileLaunch;
    private SpriteRenderer spriteRenderer;

    private int damage;
    private float aoeRadius;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float disableTimeAfterHit = 0.05f;

    [Space]
    [SerializeField] private int sortingLayerOnEnable = 20;
    [SerializeField] private int sortingLayerAfterHit = -99;
 
    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
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
        }
    }
    public void SetValues(Transform target, int _damage, float _aoeRaduis, TargetType _targetType)
    {
        dealedDamage = false;
        CancelInvoke();
        spriteRenderer.sortingOrder = sortingLayerOnEnable;

        currenttarget = target;
        bulletTarget = currenttarget.GetComponent<Enemy>().GetBulletTarget();
        damage = _damage;
        aoeRadius = _aoeRaduis;
        targetType = _targetType;

        if(targetType == TargetType.AimOnGround)
        {
            enemyPositionOnProjectileLaunch = bulletTarget.transform.position;
            transform.right = enemyPositionOnProjectileLaunch - transform.position;
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
            DealAoeDamage(enemyPositionOnProjectileLaunch, aoeRadius);
            dealedDamage = true;
            spriteRenderer.sortingOrder = sortingLayerAfterHit;
            Invoke("DisableProjectile", disableTimeAfterHit);
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
            currenttarget.GetComponent<Enemy>().TakeDamage(damage);
        }
        else
        {
            DealAoeDamage(bulletTarget.position, aoeRadius);  //* 0.01f
        }
    }
    private void DealAoeDamage(Vector3 position, float range)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(position, range, hitLayer);

        foreach (Collider2D coll in colls)
        {
            if (coll.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dealedDamage == true) return;

        if (collision.gameObject == currenttarget.gameObject)
        {

            FollowTargetDamage();
            dealedDamage = true;
            Invoke("DisableProjectile", disableTimeAfterHit);
        }
    }
    private void DisableProjectile()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
