using Marvin.PoolingSystem;
using NUnit.Framework.Internal.Execution;
using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class WorldItem : MonoBehaviour, IPoolingList
{
    public BodyObject itemInformationen;
    public int dropAmount = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 baseScale;
    private Vector3 scaledUp;
    private float scaledUpMultiplier = 1.4f;

    [Header("Other")]
    public bool testItem;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void Awake()
    {
        baseScale = transform.localScale;
        scaledUp = new Vector3(baseScale.x * scaledUpMultiplier, baseScale.y * scaledUpMultiplier, 1);
    }

    private void OnDrawGizmos()
    {
        if (itemInformationen != null)
            gameObject.name = itemInformationen.Name + "_WorldItem";
    }

    private void OnValidate()
    {
        if(itemInformationen != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemInformationen.Sprite;
        }
    }

    public void ScaleItemUp()
    {
        transform.localScale = scaledUp;
    }
    public void ScaleItemDown()
    {
        transform.localScale = baseScale;
    }
    public void ReturnToItemPool()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
