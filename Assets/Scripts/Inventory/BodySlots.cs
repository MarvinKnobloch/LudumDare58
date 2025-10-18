using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodySlots : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BodyPart bodyPart;
    public GameObject unlockObj;

    private Color transparentColor;
    private Color fullColor;

    [Space]
    public BodyObject bodyObject;

    private Image bodyImage;

    private void Awake()
    {
        bodyImage = GetComponent<Image>();
    }
    private void Start()
    {
        Color color = Color.white;
        color.a = 255;
        fullColor = color;

        Color nonColor = Color.white;
        nonColor.a = 0;
        transparentColor = nonColor;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.currentBodySlots = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IngameController.Instance.playerUI.inventory.currentBodySlots = null;
    }
    public void SlotUpdate(BodyObject obj)
    {
        UpdateSlot(obj);
    }
    public void UpdateSlot(BodyObject obj)
    {
        bodyObject = obj;
        bodyImage.color = fullColor;
        bodyImage.sprite = bodyObject.Sprite;
    }
    public void ClearSlot()
    {
        bodyObject = null;
        if(bodyImage == null) bodyImage = GetComponent<Image>();
        bodyImage.sprite = null;
        bodyImage.color = transparentColor;
    }
}
