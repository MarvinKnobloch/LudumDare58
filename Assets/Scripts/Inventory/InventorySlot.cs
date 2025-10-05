using TMPro;
using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        inventory.StartDrag(bodyObject);
    }
}
