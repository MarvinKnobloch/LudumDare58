using UnityEngine;

public class UIFollowMouse : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    void Update()
    {
        transform.position = Input.mousePosition + offset;
    }
}
