using UnityEngine;

public class FollowCurve : MonoBehaviour
{
    public AnimationCurve animationCurve; // Reference to your curve component
    public LineRenderer lineRenderer;

    public int resolution = 10; // Number of points to create on the line renderer

    void Start()
    {
        lineRenderer.positionCount = resolution;

        // Calculate and set the Line Renderer points based on the Animation Curve
        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);
            float curveValue = animationCurve.Evaluate(t);
            Vector3 pointOnCurve = new Vector3(t, curveValue, 0f); // Assuming 2D, adjust for 3D if needed
            lineRenderer.SetPosition(i, pointOnCurve);
        }
   }

}
