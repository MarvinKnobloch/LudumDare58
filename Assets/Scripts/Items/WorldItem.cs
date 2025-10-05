using Tower;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public BodyObject itemInformationen; 

    private void OnDrawGizmos()
    {
        if (itemInformationen != null)
            gameObject.name = "WorldItem_" + itemInformationen.Name;
    }
}
