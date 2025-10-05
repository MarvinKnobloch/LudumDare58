using NUnit.Framework.Interfaces;
using Tower;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryPickUp : MonoBehaviour
{

    public BodyObject ItemInformation;
    private Inventory inventory;

    private int amount = 1;

    void Start()
    {
        inventory = IngameController.Instance.playerUI.inventory;



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
                inventory.AddResource(ItemInformation.Name, amount);


                //Destroy(gameObject);

            }
        }
    }



}


