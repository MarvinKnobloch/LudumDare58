using System.Collections.Generic;
using Tower;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<BodyObject, int> resources = new Dictionary<BodyObject, int>();

    private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private GameObject prefabSlot;
    [SerializeField] private int slotAmount = 28;

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
        if (resources.ContainsKey(bodyObject))
        {
            resources[bodyObject] += amount;
            slots[resources[bodyObject]].SetAmount(resources[bodyObject], bodyObject.Sprite);
        }

        else
        {
            resources[bodyObject] = amount;
            slots[resources[bodyObject]].SetAmount(resources[bodyObject], bodyObject.Sprite);
        }

        Debug.Log($"Added {amount} {bodyObject} to inventory. Total: {resources[bodyObject]}");
    }

}
