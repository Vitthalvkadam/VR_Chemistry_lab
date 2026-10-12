using UnityEngine;

public class Liquid : MonoBehaviour
{
    [Header("Liquid Information")]
    [SerializeField] private string liquidName = "Test Liquid";

    [Range(0f, 14f)]
    [SerializeField] private float pH = 7.0f;

    [Header("Liquid Appearance")]
    [SerializeField] private Mesh liquidMesh;
    [SerializeField] private Material liquidMaterial;

    [Header("Wobble")]
    [SerializeField] private bool enableWobble = false;
    [SerializeField] private float wobbleAmount = 0.02f;
    [SerializeField] private float wobbleSpeed = 2f;
    [SerializeField] private float wobbleFrequency = 3f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Mesh originalMesh;
    private Mesh runtimeMesh;

    public float PH => pH;
    public string LiquidName => liquidName;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Assign mesh if one was provided
        if (liquidMesh != null && meshFilter != null)
        {
            meshFilter.sharedMesh = liquidMesh;
        }

        // Assign material if one was provided
        if (liquidMaterial != null && meshRenderer != null)
        {
            meshRenderer.material = liquidMaterial;
        }

        // Prepare runtime mesh for wobble
        if (enableWobble &&
            meshFilter != null &&
            meshFilter.sharedMesh != null)
        {
            originalMesh = meshFilter.sharedMesh;

            runtimeMesh = Instantiate(originalMesh);
            runtimeMesh.name = originalMesh.name + "_Runtime";

            meshFilter.mesh = runtimeMesh;
        }

        Debug.Log(
            "Liquid initialized: " +
            liquidName +
            " | pH = " +
            pH
        );
    }

    private void Update()
    {
        if (enableWobble)
        {
            WobbleLiquid();
        }
    }

    private void WobbleLiquid()
    {
        if (originalMesh == null || runtimeMesh == null)
            return;

        Vector3[] originalVertices = originalMesh.vertices;
        Vector3[] vertices = runtimeMesh.vertices;

        float time = Time.time * wobbleSpeed;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 original = originalVertices[i];

            float height = Mathf.InverseLerp(
                -0.5f,
                0.5f,
                original.y
            );

            float wobbleX =
                Mathf.Sin(
                    original.y * wobbleFrequency + time
                )
                * wobbleAmount
                * height;

            float wobbleZ =
                Mathf.Cos(
                    original.y * wobbleFrequency + time * 1.2f
                )
                * wobbleAmount
                * height;

            vertices[i] = original;

            vertices[i].x += wobbleX;
            vertices[i].z += wobbleZ;
        }

        runtimeMesh.vertices = vertices;
        runtimeMesh.RecalculateNormals();
        runtimeMesh.RecalculateBounds();
    }

    public float GetPH()
    {
        return pH;
    }
}