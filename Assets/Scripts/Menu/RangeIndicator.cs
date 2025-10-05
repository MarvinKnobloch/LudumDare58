using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private int segements;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = segements;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }
    public void DrawCircle(float radius)
    {
        int steps = lineRenderer.positionCount - 1;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float progress = (float)currentStep / steps;

            float currentRadian = progress * 2 * Mathf.PI;

            float xscale = Mathf.Cos(currentRadian);
            float yscale = Mathf.Sin(currentRadian);

            float x = xscale * radius;
            float y = yscale * radius;

            Vector3 currentPosition = new Vector3(x, y, 0);

            lineRenderer.SetPosition(currentStep, currentPosition);

            if (currentStep == 0)
            {
                lineRenderer.SetPosition(segements - 1, currentPosition);
            }
        }
    }
}
