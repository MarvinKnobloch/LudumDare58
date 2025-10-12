using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    void Update()
    {
        transform.position = Utility.MousePostion();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out WorldItem item))
        {
            item.ScaleItemUp();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WorldItem item))
        {
            item.ScaleItemDown();
        }
    }
}
