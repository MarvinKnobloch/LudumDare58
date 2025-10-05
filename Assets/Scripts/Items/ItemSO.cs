using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int amount = 1;

}

