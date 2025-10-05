using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> resources = new Dictionary<string, int>();

    [SerializeField] private GameObject prefabSlot;
    [SerializeField] private int slotAmount = 28;

    [SerializeField] private GameObject InventoryGrid;

    private void Awake()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            Instantiate(prefabSlot, Vector3.zero, Quaternion.identity, InventoryGrid.transform);


        }


    }

    public void AddResource(string resourceName, int amount)
    {
        if (resources.ContainsKey(resourceName))
            resources[resourceName] += amount;
        else
            resources[resourceName] = amount;

        Debug.Log($"Added {amount} {resourceName} to inventory. Total: {resources[resourceName]}");
    }

}
