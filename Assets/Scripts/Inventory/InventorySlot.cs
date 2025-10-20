using System.Collections;
using TMPro;
using Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{


    private PlayerUI playerUI;

    [HideInInspector] public Inventory inventory;
    public int slotAmount;
    public bool slotIsFull;
    public BodyObject bodyObject;


    [SerializeField] private Image inventoryIconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    private Image dragImage;

    private void Start()
    {
        playerUI = IngameController.Instance.playerUI;
    }

    public void SetValues(int amount, Sprite icon)
    {
        slotAmount += amount;
        if (slotAmount > 0)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = slotAmount.ToString();
            inventoryIconImage.enabled = true;
            inventoryIconImage.sprite = icon;
        }
        else
        {
            bodyObject = null;
            slotIsFull = false;
            slotAmount = 0;
            inventoryIconImage.enabled = false;
            amountText.gameObject.SetActive(false);
            transform.SetAsLastSibling();
        }
    }
    public void HideText()
    {
        inventoryIconImage.enabled = false;
        amountText.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        inventory.dragImage.SetActive(true);

        if (dragImage == null) dragImage = inventory.dragImage.GetComponent<Image>();
        dragImage.sprite = bodyObject.Sprite;

        playerUI.ToggleTooltipWindow(false, playerUI.itemTooltipWindow);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotAmount == 0 || bodyObject == null) return;

        if (inventory.currentBodySlots == null && inventory.canRecycle == false) dragImage.color = Color.red;
        else if(inventory.canRecycle == true)
        {
            dragImage.color = Color.green;
        }
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

        if(inventory.canRecycle == true)
        { 
            Player.Instance.UpdateSouls(Player.Instance.GetRecycleSoulsAmount());
            inventory.AddResource(bodyObject, -1);
        }
        else if(inventory.currentBodySlots != null)
        {
            if (inventory.currentBodySlots.bodyPart == bodyObject.Part)
            {
                inventory.currentBodySlots.SlotUpdate(bodyObject);

                if (bodyObject.Part == BodyPart.Weapon)
                {
                    inventory.currentWeaponSlot = bodyObject;
                }

                inventory.currentSelectedTower.OnBodyPartEquipped(inventory.currentSelectedTower, bodyObject);
                inventory.SetRangeIndicator();
                inventory.currentSelectedTower.CheckForRecipe();
                inventory.SetUpgradeTowerButton();
                inventory.SetTowerInfo();

                if (inventory.currentWeaponSlot != null)
                {
                    inventory.CheckForRecipeUIUpdate(bodyObject.Part);
                }

                IngameController.Instance.playerUI.SetToolTipWindow(bodyObject);

                inventory.AddResource(bodyObject, -1);
            }

            //desi
            if (GameManager.Instance.showTutorial)
            {
                IngameController.Instance.playerUI.tutorial.TryAdvanceHint(3);
            }
        }

        inventory.currentBodySlots = null;
        inventory.dragImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bodyObject == null) return;

        IngameController.Instance.playerUI.SetToolTipWindow(bodyObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(false, playerUI.itemTooltipWindow);
    }
}
