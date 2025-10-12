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


    [SerializeField] private TextMeshProUGUI amountText;
    private Image inventoryImage;

    private Image dragImage;

    private void Awake()
    {
        inventoryImage = GetComponent<Image>();
    }
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
                inventory.currentSelectedTower.OnBodyPartEquipped(inventory.currentSelectedTower, bodyObject);
                inventory.SetRangeIndicator();
                inventory.currentSelectedTower.CheckForRecipe();
                inventory.SetUpgradeTowerButton();
                inventory.SetTowerInfo();
            }
        }

        inventory.currentBodySlots = null;
        inventory.dragImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bodyObject == null) return;

        SetText();
        SetWindowPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerUI.ToggleTooltipWindow(false, playerUI.itemTooltipWindow);
    }

    private void SetText()
    {
        if (bodyObject == null) return;

        TextMeshProUGUI itemText = playerUI.itmeTooltipText;
        itemText.text = string.Empty;
        itemText.text += "<u><b>" + bodyObject.Name + "</u></b>\n\n";

        if (bodyObject.BonusDamage > 0) itemText.text += "<color=green>+" + bodyObject.BonusDamage + "</color> damage\n";
        else if (bodyObject.BonusDamage < 0) itemText.text += "<color=red>+" + bodyObject.BonusDamage + "</color> damage\n";

        if (bodyObject.BonusAttackSpeed > 0) itemText.text += "<color=green>+" + bodyObject.BonusAttackSpeed + "%</color> attack speed\n";
        else if (bodyObject.BonusAttackSpeed < 0) itemText.text += "<color=red>+" + bodyObject.BonusAttackSpeed + "%</color> attack speed\n";

        if (bodyObject.BonusRange > 0) itemText.text += "<color=green>+" + bodyObject.BonusRange + "</color> range\n";
        else if (bodyObject.BonusRange > 0) itemText.text += "<color=red>+" + bodyObject.BonusRange + "</color> range\n";

        if (bodyObject.BonusAoeRadius > 0) itemText.text += "<color=green>+" + bodyObject.BonusAoeRadius + "</color> range\n";
        else if (bodyObject.BonusAoeRadius > 0) itemText.text += "<color=red>+" + bodyObject.BonusAoeRadius + "</color> range\n";

        if (bodyObject.DamageScalingPercentage > 0) itemText.text += "<color=green>" + bodyObject.DamageScalingPercentage + "%</color> damage scaling\n";

        if (bodyObject.BaseAttackSpeed > 0) itemText.text += "<color=green>" + bodyObject.BaseAttackSpeed + "</color> base attack speed\n";

        if (bodyObject.RangeScalingPercentage > 0) itemText.text += "<color=green>" + bodyObject.RangeScalingPercentage + "%</color> range scaling\n";

        if (bodyObject.SlowPercentage > 0) itemText.text += "<color=green>+" + bodyObject.SlowPercentage + "%</color> slow\n";
        if (bodyObject.SlowDuration > 0) itemText.text += "<color=green>+" + bodyObject.SlowDuration + "</color> slow duration\n";

        if (bodyObject.LifeSteal == true) itemText.text += "Lifesteal on kill (<color=green>" + Player.Instance.GetLifeStealChance() + "%</color> chance)\n";

        if (bodyObject.ChanceForDoubleDamage == true) itemText.text += "Double damage (<color=green>" + Player.Instance.GetDoubleDamageChance() + "%</color> chance)\n";

        if(bodyObject.AdditionalProjectiles == 1) itemText.text += "<color=green>+" + bodyObject.AdditionalProjectiles + "</color> Projectile";
        else if (bodyObject.AdditionalProjectiles > 1) itemText.text += "<color=green>+" + bodyObject.AdditionalProjectiles + "</color> Projectiles";
    }

    private void SetWindowPosition()
    {
        //Braucht ein Frame um die Height vom ContenSizeFitter zu setzen. Scale wird für den einen Frame auf 0 gesetzt damit dann das FEnster nicht springen sieht
        StartCoroutine(SetWindowPostionAfterResize());
    }
    IEnumerator SetWindowPostionAfterResize()
    {
        yield return null;
        GameObject window = playerUI.itemTooltipWindow;
        playerUI.ToggleTooltipWindow(true, window);
        ////window.transform.localScale = new Vector3(1, 1, 1);
        ////Ist keine Formel, einfach ein bisschen ausprobiert was passt
        float widthOffset = Screen.width / 9.5f;  //7

        float heigthoffset = ((Screen.height * 0.5f) - Input.mousePosition.y) / 70;              //Bestimmt ob das Fenster nach unten oder oben geht.

        ////if (heigthoffset > 0) heigthoffset += tooltipRect.rect.height * rectHeightMultiplier;    //Wie weit das Fenster nach unten/oben geht basierend auf der Größe
        ////else heigthoffset -= tooltipRect.rect.height * rectHeightMultiplier;

        ////links oder rechts von mousePosition          1.35f = 35% vom rechten Bildschirmrand
        if (Screen.width / Input.mousePosition.x > 1.43f) window.transform.position = Input.mousePosition + new Vector3(widthOffset, heigthoffset, 0);
        else window.transform.position = Input.mousePosition + new Vector3(widthOffset * -1, heigthoffset, 0);
    }
}
