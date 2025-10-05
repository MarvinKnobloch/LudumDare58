using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryItemDrag : MonoBehaviour
{

    public ItemSO ItemInformation;
    private Inventory inventory;

    void Start()
    {
        inventory = Object.FindFirstObjectByType<Inventory>();
    }
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Wirde angeklickt");
                inventory.AddResource(ItemInformation.itemName, ItemInformation.amount);

            }
        }
    }



}


