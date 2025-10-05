using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> resources = new Dictionary<string, int>();



    public void AddResource(string resourceName, int amount)
    {
        if (resources.ContainsKey(resourceName))
            resources[resourceName] += amount;
        else
            resources[resourceName] = amount;

        Debug.Log($"Added {amount} {resourceName} to inventory. Total: {resources[resourceName]}");
    }

}
