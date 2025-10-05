using System;
using System.Collections.Generic;
using Tower;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<BodyObject, InventoryInfo> resources = new Dictionary<BodyObject, InventoryInfo>();

    private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private GameObject prefabSlot;
    [SerializeField] private int slotAmount = 28;

    private int currentInventoryPosition;
    [SerializeField] private GameObject InventoryGrid;

    private void Awake()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            InventorySlot slot = Instantiate(prefabSlot, Vector3.zero, Quaternion.identity, InventoryGrid.transform).GetComponent<InventorySlot>();
            slots.Add(slot);
            slots[i].HideText();
        }
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

            //resources[bodyObject].slotPosition = GetEmptySlot();
            //resources[bodyObject].amount += amount;
            slots[resources[bodyObject].slotPosition].SetAmount(resources[bodyObject].slotAmount, bodyObject.Sprite);
        }

        else
        {
            resources[bodyObject].slotAmount += amount;
            slots[resources[bodyObject].slotPosition].SetAmount(resources[bodyObject].slotAmount, bodyObject.Sprite);
        }

        Debug.Log($"Added {amount} {bodyObject} to inventory. Total: {resources[bodyObject]}");
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
    }

    [Serializable]
    public class InventoryInfo
    {
        public int slotPosition;
        public int slotAmount;
    }

}
