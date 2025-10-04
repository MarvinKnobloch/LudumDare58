using UnityEngine;
using UnityEngine.InputSystem;

public static class Utility
{
    public static bool LayerCheck(Collider2D collisionObj, LayerMask layersToCheck)
    {
        if (((1 << collisionObj.gameObject.layer) & layersToCheck) != 0)
        {
            return true;
        }
        else
            return false;
    }
    public static Vector3 MousePostion()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        return mousePosition;
    }
}