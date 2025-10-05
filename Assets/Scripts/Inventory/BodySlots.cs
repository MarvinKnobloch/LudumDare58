using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodySlots : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BodyPart bodyPart;

    [Space]
    public BodyObject bodyObject;

    private Image bodyImage;

    private void Awake()
    {
        bodyImage = GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.bodySlots = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.bodySlots = null;
    }
    public void SetBodySlot(BodyObject obj)
    {
        //RemoveOldValues

        bodyObject = obj;
        bodyImage.sprite = bodyObject.Sprite;

        //reduce amount
        //SetValues
    }
}
