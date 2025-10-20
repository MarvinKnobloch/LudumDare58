using Tower;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecycleBin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.canRecycle = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.canRecycle = false;
    }
}
