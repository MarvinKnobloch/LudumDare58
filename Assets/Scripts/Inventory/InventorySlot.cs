using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private int slotAmount;
    [SerializeField] private TextMeshProUGUI amountText;
    private Image inventoryImage;

    private void Awake()
    {
        inventoryImage = GetComponent<Image>();
    }

    public void SetAmount(int amount, Sprite icon)
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
}
