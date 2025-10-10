using GifImporter;
using Marvin.PoolingSystem;
using UnityEngine;

public class DealDmgOnEnter : MonoBehaviour, IPoolingList
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float lifeTime;

    [HideInInspector] public int damage;
    [HideInInspector] public bool baseScalingSaved;
    [HideInInspector] public Vector3 baseScaling;

    public bool ResetRotation = false;

    private GifPlayer _gifPlayer;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }
    private void OnEnable()
    {
        CancelInvoke();
        Invoke("DisableObject", lifeTime);

        TryGetComponent(out _gifPlayer);
        if (_gifPlayer != null)
        {
            _gifPlayer.Reset();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerCheck(collision, hitLayer))
        {
            if (collision.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    private void DisableObject()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
