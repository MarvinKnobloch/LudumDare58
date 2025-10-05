using TMPro;
using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Inventory inventory;
    public int slotAmount;
    public BodyObject bodyObject;


    [SerializeField] private TextMeshProUGUI amountText;
    private Image inventoryImage;


    private void Awake()
    {
        inventoryImage = GetComponent<Image>();
    }

    public void SetValues(int amount, Sprite icon)
    {
        slotAmount = amount;
        if (amount > 0)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = slotAmount.ToString();
            inventoryImage.sprite  = icon;
        }
        else
        {
            amountText.gameObject.SetActive(false);
            inventoryImage.sprite = null;
        }
    }
    public void HideText()
    {
        amountText.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        inventory.dragImage.SetActive(true);
        inventory.dragImage.GetComponent<Image>().sprite = bodyObject.Sprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        inventory.dragImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        inventory.dragImage.SetActive(false);
    }
}
