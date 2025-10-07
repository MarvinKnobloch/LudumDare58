using Marvin.PoolingSystem;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolingList
{
    private Transform currenttarget;
    private Transform bulletTarget;
    private Vector3 direction;
    private bool targetHasDied;
    private bool dealedDamage;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask hitLayer;
    private int damage;

    //Area
    private float aoeRadius;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    void Update()
    {
        SingleTarget();
    }
    public void SetValues(Transform target, int _damage, float _aoeRaduis)
    {
        currenttarget = target;
        bulletTarget = currenttarget.GetComponent<Enemy>().GetBulletTarget();
        damage = _damage;
        aoeRadius = _aoeRaduis;
        dealedDamage = false;
        CancelInvoke();
    }
    private void SingleTarget()
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
    private void BulletNoTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, projectileSpeed * Time.deltaTime);
        if (targetHasDied == false)
        {
            targetHasDied = true;
            Invoke("DisableProjectile", 0.5f);
        }
    }
    private void DealDamageOnTargetPosition()
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

            Debug.Log("collide");
            DealDamageOnTargetPosition();
            dealedDamage = true;
            Invoke("DisableProjectile", 0.05f);
        }
    }
    private void DisableProjectile()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
