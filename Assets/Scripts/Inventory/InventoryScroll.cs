using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;

public class InventoryScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 0.08f;
    void Update()
    {
        if (Mouse.current != null)
        {


            float scroll = Mouse.current.scroll.ReadValue().y; //da hat mir GPT geholfen weil das irgendwie mit dem input net ging


            if (scroll != 0f)
            {
                scrollRect.verticalNormalizedPosition += scroll * scrollSpeed;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }

        }
    }
}
