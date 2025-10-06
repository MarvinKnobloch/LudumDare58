using System;
using System.Collections.Generic;
using TMPro;
using Tower;
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

    public void AddResource(BodyObject bodyObject, int amount)
    {
        if (resources.ContainsKey(bodyObject) == false)
        {
            int test = GetEmptySlot();
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
            if (slots[i].slotAmount == 0) return i;
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
        SetSlots(accessoiresSlot, currentSelectedTower.accessoires);
        SetSlots(headSlot, currentSelectedTower.head);
        SetSlots(armsSlot, currentSelectedTower.arms);
        SetSlots(bodySlot, currentSelectedTower.body);
        SetSlots(weaponSlot, currentSelectedTower.weapon);

        SetRangeIndicator();
        SetSuccessText();

        bodySlotsUI.SetActive(true);
    }
    private void SetSlots(BodySlots bodySlots , BodyObject bodyObject)
    {
        if (bodyObject != null)
        {
            bodySlots.UpdateSlot(bodyObject);
        }
        else
        {
            bodySlots.ClearSlot();
        }
    }
    public void SetRangeIndicator()
    {
        rangeIndicator.gameObject.transform.position = currentSelectedTower.gameObject.transform.position;
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.DrawCircle(currentSelectedTower._currentRange);
    }
    public void SetSuccessText()
    {
        successPercantageText.transform.parent.gameObject.SetActive(true);
        successPercantageText.text = "<color=green>" + currentSelectedTower.recipeMatchPercent + "</color>%";
    }

    [Serializable]
    public class InventoryInfo
    {
        public int slotPosition;
        public int slotAmount;
    }

}
