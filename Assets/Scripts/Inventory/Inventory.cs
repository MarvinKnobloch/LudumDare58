using System;
using System.Collections.Generic;
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

    [Space] 
    private GameObject dragImage;

    private void Awake()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            CreateNewSlot();
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


            slots[resources[bodyObject].slotPosition].bodyObject = bodyObject;
            slots[resources[bodyObject].slotPosition].SetValues(resources[bodyObject].slotAmount, bodyObject.Sprite);
        }

        else
        {
            resources[bodyObject].slotAmount += amount;

            slots[resources[bodyObject].slotPosition].SetValues(resources[bodyObject].slotAmount, bodyObject.Sprite);
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
        slot.inventory = this;
        slot.HideText();
    }
    public void StartDrag(BodyObject bodyObject)
    {
        dragImage.SetActive(true);
        dragImage.GetComponent<DragImage>().SetDragImage(bodyObject);
    }

    [Serializable]
    public class InventoryInfo
    {
        public int slotPosition;
        public int slotAmount;
    }

}
