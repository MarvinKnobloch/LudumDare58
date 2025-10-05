using Tower;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DragImage : MonoBehaviour
{
    private BodyObject bodyObject;
    private Image image;
    private Controls controls;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
        image = GetComponent<Image>();
    }
    private void Update()
    {
        if (controls.Player.Confirm.IsPressed())
        {
            transform.position = Utility.MousePostion();
        }
        else
        {
            //CheckForDrop;
            bodyObject = null;
            gameObject.SetActive(false);
        }
    }

    public void SetDragImage(BodyObject obj)
    {
        if(image == null) image = GetComponent<Image>();
        bodyObject = obj;
        image.sprite = bodyObject.Sprite;
    }
}
