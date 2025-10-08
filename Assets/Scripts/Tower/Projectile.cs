using System.Collections;
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
    private float timer;

    private int damage;
    private float aoeRadius;

    //Swing
    [SerializeField] private float startRotation;
    [SerializeField] private float endRotation;
    private float percentage;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float disableTimeAfterHit = 0.05f;

    [Space]
    [SerializeField] private int sortingLayerOnEnable = 20;
    [SerializeField] private int sortingLayerAfterHit = -99;        //Used for AimOnGroundOnly

 
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
            case TargetType.Swing:
                Swing();
                break;
        }
    }
    public void SetValues(Transform target, int _damage, float _aoeRaduis, float _range, TargetType _targetType)
    {
        dealedDamage = false;
        CancelInvoke();
        StopAllCoroutines();
        spriteRenderer.sortingOrder = sortingLayerOnEnable;
        timer = 0;
        percentage = 0;

        currenttarget = target;
        bulletTarget = currenttarget.GetComponent<Enemy>().GetBulletTarget();
        damage = _damage;
        aoeRadius = _aoeRaduis;
        targetType = _targetType;

        switch (targetType)
        {
            case TargetType.AimOnGround:
                enemyPositionOnProjectileLaunch = bulletTarget.transform.position;
                transform.right = enemyPositionOnProjectileLaunch - transform.position;
                break;
            case TargetType.Swing:
                transform.localScale = new Vector3(0.5f * _range, 0.5f * _range, 1);

                Vector3 targ = bulletTarget.position;
                Vector3 objectPos = transform.position;
                targ.x = targ.x - objectPos.x;
                targ.y = targ.y - objectPos.y;

                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
                startRotation = angle + (20 * aoeRadius);
                endRotation = angle - (20 * aoeRadius);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, startRotation));

                StartCoroutine(SwingDelay());
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
        percentage = timer / projectileSpeed;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(startRotation, endRotation, percentage)));

        if (timer > projectileSpeed)
        {
            DisableProjectile();
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
