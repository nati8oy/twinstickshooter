using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAnimationCurve : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public AnimationCurve animationCurve;
    public int resolution = 100; // Number of points on the Line Renderer

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
