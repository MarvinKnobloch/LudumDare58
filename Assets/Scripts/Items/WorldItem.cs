using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemSO itemInformationen; 

    private void OnDrawGizmos()
    {
        if (itemInformationen != null)
            gameObject.name = "WorldItem_" + itemInformationen.itemName;
    }
}
