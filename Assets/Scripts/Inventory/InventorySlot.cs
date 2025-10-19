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
        if (amount > 0)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = slotAmount.ToString();
            inventoryIconImage.enabled = true;
            inventoryIconImage.sprite = icon;
        }
        else
        {
            amountText.text = slotAmount.ToString();
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
                    Debug.Log("recipeUpdate");
                    inventory.CheckForRecipeUIUpdate(bodyObject.Part);
                }
            }

            //desi
            IngameController.Instance.playerUI.tutorial.TryAdvanceHint(3);

        }

        inventory.currentBodySlots = null;
        inventory.dragImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bodyObject == null) return;

        SetWindow();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(false, playerUI.itemTooltipWindow);
    }

    private void SetWindow()
    {
        if (bodyObject == null) return;

        playerUI.ToggleTooltipWindow(true, playerUI.itemTooltipWindow);

        TextMeshProUGUI itemText = playerUI.itmeTooltipText;
        itemText.text = string.Empty;
        itemText.text += "<u><b>" + bodyObject.Name + "</u></b>\n\n";
        bool baseStat = false;
        bool towerStat = false;

        if (bodyObject.BonusDamage != 0) baseStat = true;
        if (bodyObject.BonusDamage > 0) itemText.text += "<color=green>+" + bodyObject.BonusDamage + "</color> Damage\n";
        else if (bodyObject.BonusDamage < 0) itemText.text += "<color=red>" + bodyObject.BonusDamage + "</color> Damage\n";

        if (bodyObject.BonusAttackSpeed != 0) baseStat = true;
        if (bodyObject.BonusAttackSpeed > 0) itemText.text += "<color=green>+" + bodyObject.BonusAttackSpeed + "%</color> Attack Speed\n";
        else if (bodyObject.BonusAttackSpeed < 0) itemText.text += "<color=red>" + bodyObject.BonusAttackSpeed + "%</color> Attack Speed\n";

        if (bodyObject.BonusRange != 0) baseStat = true;
        if (bodyObject.BonusRange > 0) itemText.text += "<color=green>+" + bodyObject.BonusRange + "</color> Range\n";
        else if (bodyObject.BonusRange > 0) itemText.text += "<color=red>" + bodyObject.BonusRange + "</color> Range\n";

        if (bodyObject.BonusAoeRadius != 0) baseStat = true;
        if (bodyObject.BonusAoeRadius > 0) itemText.text += "<color=green>+" + bodyObject.BonusAoeRadius + "</color> Area Size\n";
        else if (bodyObject.BonusAoeRadius > 0) itemText.text += "<color=red>" + bodyObject.BonusAoeRadius + "</color> Area Size\n";

        if (baseStat == true) itemText.text += "\n";

        if (bodyObject.DamageScalingPercentage > 0) 
        {
            towerStat = true;
            itemText.text += "<color=green>" + bodyObject.DamageScalingPercentage + "%</color> Damage Scaling\n"; 
        }

        if (bodyObject.BaseAttackSpeed > 0)
        {
            towerStat = true;
            itemText.text += "<color=green>" + bodyObject.BaseAttackSpeed + "</color> Base Attack Speed\n"; 
        }

        if (bodyObject.RangeScalingPercentage > 0)
        {
            towerStat = true;
            itemText.text += "<color=green>" + bodyObject.RangeScalingPercentage + "%</color> Range Scaling\n"; 
        }

        if (towerStat == true) itemText.text += "\n";

        if (bodyObject.SlowPercentage > 0) itemText.text += "<color=green>+" + bodyObject.SlowPercentage + "%</color> Slow\n";
        if (bodyObject.SlowDuration > 0) itemText.text += "<color=green>+" + bodyObject.SlowDuration + "</color> Slow Duration\n";

        if (bodyObject.LifeSteal == true) itemText.text += "Lifesteal on kill (<color=green>" + Player.Instance.GetLifeStealChance() + "%</color> chance)\n";

        if (bodyObject.ChanceForDoubleDamage == true) itemText.text += "Double Damage (<color=green>" + Player.Instance.GetDoubleDamageChance() + "%</color> chance)\n";

        if(bodyObject.AdditionalProjectiles == 1) itemText.text += "<color=green>+" + bodyObject.AdditionalProjectiles + "</color> Projectile";
        else if (bodyObject.AdditionalProjectiles > 1) itemText.text += "<color=green>+" + bodyObject.AdditionalProjectiles + "</color> Projectiles";
    }
}
