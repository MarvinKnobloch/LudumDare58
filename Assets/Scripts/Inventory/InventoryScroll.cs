using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;

public class InventoryScroll : MonoBehaviour
{



    public ScrollRect scrollRect;



    void Update()
    {
        if (Mouse.current != null)
        {


            float scroll = Mouse.current.scroll.ReadValue().y; //da hat mir GPT geholfen weil das irgendwie mit dem input net ging


            if (scroll != 0f)
            {
                scrollRect.verticalNormalizedPosition += scroll * -0.003f;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }

        }
    }
}
