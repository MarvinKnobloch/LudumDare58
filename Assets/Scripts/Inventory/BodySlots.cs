using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodySlots : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BodyPart bodyPart;
    [SerializeField] private Sprite noItemImage;

    [Space]
    public BodyObject bodyObject;

    private Image bodyImage;

    private void Awake()
    {
        bodyImage = GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.currentBodySlots = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.currentBodySlots = null;
    }
    public void TowerUpdate(TowerBase towerBase, BodyObject obj)
    {
        towerBase.OnBodyPartEquipped(towerBase, obj);

        UpdateSlot(obj);
    
        //reduce souls
    }
    public void UpdateSlot(BodyObject obj)
    {
        bodyObject = obj;
        bodyImage.sprite = bodyObject.Sprite;
    }
    public void ClearSlot()
    {
        bodyObject = null;
        if(bodyImage == null) bodyImage = GetComponent<Image>();
        bodyImage.sprite = noItemImage;
    }
}
