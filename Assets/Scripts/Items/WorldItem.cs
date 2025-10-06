using Tower;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public BodyObject itemInformationen;
    public int dropAmount = 1;

    private void OnDrawGizmos()
    {
        if (itemInformationen != null)
            gameObject.name = itemInformationen.Name + "_WorldItem";
    }
}
