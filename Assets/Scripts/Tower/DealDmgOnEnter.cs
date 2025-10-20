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

    [HideInInspector] public float slowPercentage;
    [HideInInspector] public float slowDuration;
    [HideInInspector] public bool canDoDoubleDamage;
    [HideInInspector] public bool lifeSteal;

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
                enemy.TakeDamage(damage, lifeSteal);

                if (slowPercentage > 0 && enemy.gameObject.activeSelf == true) enemy.DoSlow(slowPercentage, slowDuration);
            }
        }
    }
    private void DisableObject()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
