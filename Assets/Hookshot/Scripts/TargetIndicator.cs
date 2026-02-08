using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [Header("Decal Settings")]
    [SerializeField] private float decalSize = 1.5f;
    [SerializeField] private float groundOffset = 0.05f;
    [SerializeField] private float groundRaycastDistance = 50f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Appearance")]
    [SerializeField] private Color decalColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField] private float ringThickness = 0.15f;

    private GameObject decalQuad;
    private bool isVisible;

    private void Awake()
    {
        CreateDecalQuad();
        Hide();
    }

    private void CreateDecalQuad()
    {
        decalQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        decalQuad.name = "TargetingDecal";
        decalQuad.transform.SetParent(transform);

        // Rotate to lie flat on the ground (quad default faces +Z, rotate to face +Y)
        decalQuad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        decalQuad.transform.localScale = new Vector3(decalSize, decalSize, 1f);

        // Remove the collider that CreatePrimitive adds
        Collider col = decalQuad.GetComponent<Collider>();
        if (col != null)
            Destroy(col);

        // Create material with procedural circle texture
        MeshRenderer renderer = decalQuad.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Unlit/Transparent"));
        mat.mainTexture = GenerateCircleTexture(128);
        mat.color = decalColor;
        renderer.material = mat;

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private Texture2D GenerateCircleTexture(int resolution)
    {
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        float center = resolution * 0.5f;
        float outerRadius = center;
        float innerRadius = center * (1f - ringThickness);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));

                if (dist >= innerRadius && dist <= outerRadius)
                {
                    // Ring band — fade at the edges for a smooth look
                    float edgeFade = 1f;
                    float fadeWidth = 2f;
                    if (dist > outerRadius - fadeWidth)
                        edgeFade = (outerRadius - dist) / fadeWidth;
                    if (dist < innerRadius + fadeWidth)
                        edgeFade = Mathf.Min(edgeFade, (dist - innerRadius) / fadeWidth);

                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(edgeFade)));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return tex;
    }

    /// <summary>
    /// Show the decal on the ground beneath the given world position.
    /// </summary>
    public void ShowAtTarget(GameObject target)
    {
        if (target == null) { Hide(); return; }

        Vector3 targetPosition = target.transform.position;

        // Raycast down from above the target to find the ground
        Vector3 rayOrigin = targetPosition + Vector3.up * 2f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit groundHit, groundRaycastDistance, groundLayer))
        {
            decalQuad.transform.position = groundHit.point + Vector3.up * groundOffset;
            // Align to ground normal for slopes
            decalQuad.transform.rotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal) * Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            // No ground found; place directly below the target
            decalQuad.transform.position = new Vector3(targetPosition.x, targetPosition.y - 0.5f, targetPosition.z);
            decalQuad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        if (!isVisible)
        {
            decalQuad.SetActive(true);
            isVisible = true;
        }
    }

    /// <summary>
    /// Hide the decal.
    /// </summary>
    public void Hide()
    {
        if (isVisible || (decalQuad != null && decalQuad.activeSelf))
        {
            decalQuad.SetActive(false);
            isVisible = false;
        }
    }
}
