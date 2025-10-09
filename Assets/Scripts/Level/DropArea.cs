using UnityEngine;

public class DropAreaConfiner : MonoBehaviour
{
    private static DropAreaConfiner instance;
    private BoxCollider2D area;

    public static DropAreaConfiner Instance => instance;

    private void Awake()
    {
        instance = this;
        area = GetComponent<BoxCollider2D>();
    }

    public Vector3 ClampToArea(Vector3 position, float margin = 0.05f)
    {
        if (area == null)
            return position;

        Bounds b = area.bounds;
        float clampedX = Mathf.Clamp(position.x, b.min.x + margin, b.max.x - margin);
        float clampedY = Mathf.Clamp(position.y, b.min.y + margin, b.max.y - margin);
        return new Vector3(clampedX, clampedY, position.z);
    }
}
