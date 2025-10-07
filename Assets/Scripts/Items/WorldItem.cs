using Tower;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public BodyObject itemInformationen;
    public int dropAmount = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;

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
}
