using UnityEngine;

public class ThrowOverride : MonoBehaviour
{
    [Tooltip("Override the throw angle for this object (degrees). Higher = more arc, lower = flatter.")]
    public float throwAngle = 10f;

    [Tooltip("Override the throw force for this object. Set to 0 to use the default hookshot throw force.")]
    public float throwForce = 0f;
}
