using Tower;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public BodyObject itemInformationen;
    public int dropAmount;

    private void OnDrawGizmos()
    {
        if (itemInformationen != null)
            gameObject.name = "WorldItem_" + itemInformationen.Name;
    }
}
