using System;
using System.Collections.Generic;
using TMPro;
using Tower;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Dictionary<BodyObject, InventoryInfo> resources = new Dictionary<BodyObject, InventoryInfo>();

    private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private GameObject prefabSlot;
    [SerializeField] private int slotAmount = 28;

    [SerializeField] private GameObject InventoryGrid;

    [Header("Equip Items")]
    public GameObject dragImage;
    public GameObject bodySlotsUI;
    [SerializeField] private BodySlots accessoiresSlot;
    [SerializeField] private BodySlots headSlot;
    [SerializeField] private BodySlots armsSlot;
    [SerializeField] private BodySlots bodySlot;
    [SerializeField] private BodySlots weaponSlot;
    public RangeIndicator rangeIndicator;
    [SerializeField] private TextMeshProUGUI successPercantageText;

    [HideInInspector] public BodySlots currentBodySlots;
    [HideInInspector] public TowerBase currentSelectedTower;

    private void Awake()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            CreateNewSlot();
        }
        bodySlotsUI.SetActive(false);
        successPercantageText.transform.parent.gameObject.SetActive(false);
    }
    private void Start()
    {
        accessoiresSlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetAccessoiresCosts().ToString();
        headSlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetHeadCosts().ToString();
        armsSlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetArmsCosts().ToString();
        bodySlot.unlockObj.GetComponentInChildren<TextMeshProUGUI>().text = Player.Instance.GetBodyCosts().ToString();
    }

    public void AddResource(BodyObject bodyObject, int amount)
    {
        if (resources.ContainsKey(bodyObject) == false)
        {
            resources.Add(bodyObject, new InventoryInfo()
            {
                slotPosition = GetEmptySlot(),
                slotAmount = amount
            });

            slots[resources[bodyObject].slotPosition].bodyObject = bodyObject;
            slots[resources[bodyObject].slotPosition].SetValues(amount, bodyObject.Sprite);
        }

        else
        {
            resources[bodyObject].slotAmount += amount;

            slots[resources[bodyObject].slotPosition].SetValues(amount, bodyObject.Sprite);
        }
    }
    private int GetEmptySlot()
    {
        for(int i = 0;i < slots.Count; i++)
        {
            if (slots[i].slotIsFull == false)
            {
                slots[i].slotIsFull = true;
                return i;
            }
        }
        Debug.Log("No more empty slots");
        CreateNewSlot();
        return slots.Count - 1;
    }
    private void CreateNewSlot()
    {
        InventorySlot slot = Instantiate(prefabSlot, Vector3.zero, Quaternion.identity, InventoryGrid.transform).GetComponent<InventorySlot>();
        slots.Add(slot);
        slot.inventory = this;
        slot.HideText();
    }
    public void SetCurrentTower(TowerBase tower)
    {
        currentSelectedTower = tower;
        if (currentSelectedTower.isRecipeTower == false)
        {
            SetSlots(accessoiresSlot, currentSelectedTower.accessoires);
            SetSlots(headSlot, currentSelectedTower.head);
            SetSlots(armsSlot, currentSelectedTower.arms);
            SetSlots(bodySlot, currentSelectedTower.body);

            SetSlots(weaponSlot, currentSelectedTower.weapon);
            weaponSlot.gameObject.SetActive(true);
        }
        else
        {
            if (currentSelectedTower.accessoiresSlotUnlocked) SetSlots(accessoiresSlot, currentSelectedTower.accessoires);
            else SetLockedState(accessoiresSlot);
            if (currentSelectedTower.headSlotUnlocked) SetSlots(headSlot, currentSelectedTower.head);
            else SetLockedState(headSlot);
            if (currentSelectedTower.armsSlotUnlocked) SetSlots(armsSlot, currentSelectedTower.arms);
            else SetLockedState(armsSlot);
            if (currentSelectedTower.bodySlotUnlocked) SetSlots(bodySlot, currentSelectedTower.body);
            else SetLockedState(bodySlot);

            SetLockedState(weaponSlot);
            weaponSlot.gameObject.SetActive(false);
        }



        SetRangeIndicator();
        SetSuccessText();

        bodySlotsUI.SetActive(true);
    }
    private void SetSlots(BodySlots bodySlots , BodyObject bodyObject)
    {
        bodySlots.unlockObj.SetActive(false);
        bodySlots.enabled = true;

        if (bodyObject != null)
        {
            bodySlots.UpdateSlot(bodyObject);
        }
        else
        {
            bodySlots.ClearSlot();
        }
    }
    private void SetLockedState(BodySlots bodySlots)
    {
        bodySlots.unlockObj.SetActive(true);
        bodySlots.enabled = false;
        bodySlots.ClearSlot();
    }
    public void UnlockSlot(BodySlots bodySlot)
    {
        int currentSouls = Player.Instance.GetCurrentSouls();
        int costs = 0;

        switch (bodySlot.bodyPart)
        {
            case BodyPart.Accessory:
                costs = Player.Instance.GetAccessoiresCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.accessoiresSlotUnlocked = true;
                break;
            case BodyPart.Head:
                costs = Player.Instance.GetHeadCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.headSlotUnlocked = true;
                break;
            case BodyPart.Arm:
                costs = Player.Instance.GetArmsCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.armsSlotUnlocked = true;
                break;
            case BodyPart.Torso:
                costs = Player.Instance.GetBodyCosts();
                if (currentSouls < costs) return;
                currentSelectedTower.bodySlotUnlocked = true;
                break;
            case BodyPart.Weapon:
                break;
        }

        bodySlot.unlockObj.SetActive(false);
        bodySlot.enabled = true;
        Player.Instance.UpdateSouls(-costs);
    }

    public void SetRangeIndicator()
    {
        rangeIndicator.gameObject.transform.position = currentSelectedTower.gameObject.transform.position;
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.DrawCircle(currentSelectedTower.GetTowerRange());
    }
    public void SetSuccessText()
    {
        if (currentSelectedTower.isRecipeTower) successPercantageText.transform.parent.gameObject.SetActive(false);
        else
        {
            successPercantageText.transform.parent.gameObject.SetActive(true);
            successPercantageText.text = "<color=green>" + currentSelectedTower.recipeMatchPercent + "</color>%";
        }
    }

    [Serializable]
    public class InventoryInfo
    {
        public int slotPosition;
        public int slotAmount;
    }

}
