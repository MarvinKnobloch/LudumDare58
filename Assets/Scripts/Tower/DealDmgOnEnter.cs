using Marvin.PoolingSystem;
using UnityEngine;

public class DealDmgOnEnter : MonoBehaviour, IPoolingList
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float lifeTime;

    [HideInInspector] public int damage;
    [HideInInspector] public bool baseScalingSaved;
    [HideInInspector] public Vector3 baseScaling;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }
    private void OnEnable()
    {
        CancelInvoke();
        Invoke("DisableObject", lifeTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerCheck(collision, hitLayer))
        {
            if(collision.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }
    private void DisableObject()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
