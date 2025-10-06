using TMPro;
using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Inventory inventory;
    public int slotAmount;
    public bool slotIsFull;
    public BodyObject bodyObject;


    [SerializeField] private TextMeshProUGUI amountText;
    private Image inventoryImage;

    private Image dragImage;

    private void Awake()
    {
        inventoryImage = GetComponent<Image>();
    }

    public void SetValues(int amount, Sprite icon)
    {
        slotAmount += amount;
        if (amount > 0)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = slotAmount.ToString();
            inventoryImage.sprite = icon;
        }
        else
        {
            amountText.text = slotAmount.ToString();
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

        if (dragImage == null) dragImage = inventory.dragImage.GetComponent<Image>();
        dragImage.sprite = bodyObject.Sprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        if (inventory.currentBodySlots == null) dragImage.color = Color.red;
        else
        {
            if (inventory.currentBodySlots.bodyPart == bodyObject.Part)
            {
                dragImage.color = Color.green;
            }
            else
            {
                dragImage.color = Color.red;
            }
        }

        dragImage.gameObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        if(inventory.currentBodySlots != null)
        {
            if (inventory.currentBodySlots.bodyPart == bodyObject.Part)
            {
                inventory.AddResource(bodyObject, -1);
                inventory.currentBodySlots.SlotUpdate(bodyObject);
                inventory.currentSelectedTower.OnBodyPartEquipped(inventory.currentSelectedTower, bodyObject);
                inventory.SetRangeIndicator();
                inventory.currentSelectedTower.CheckForRecipe();
                inventory.SetSuccessText();
            }
        }

        inventory.currentBodySlots = null;
        inventory.dragImage.SetActive(false);
    }
}
