using UnityEngine;

public class liquidControler : MonoBehaviour
{
    [Header("Liquid Appearance")]
    [SerializeField] private Color liquidColor = new Color(0.2f, 0.7f, 1f, 0.65f);

    [Header("Wobble")]
    [SerializeField] private float wobbleAmount = 0.02f;
    [SerializeField] private float wobbleSpeed = 3f;

    private Renderer liquidRenderer;
    private Material liquidMaterial;

    private Vector3 originalPosition;
    private float time;

    void Start()
    {
        liquidRenderer = GetComponent<Renderer>();

        // Creates an instance of the material for this liquid only
        liquidMaterial = liquidRenderer.material;

        originalPosition = transform.localPosition;

        SetLiquidColor(liquidColor);
    }

    void Update()
    {
        WobbleLiquid();
    }

    public void SetLiquidColor(Color newColor)
    {
        liquidColor = newColor;

        if (liquidMaterial != null)
        {
            liquidMaterial.color = liquidColor;
        }
    }

    private void WobbleLiquid()
    {
        time += Time.deltaTime * wobbleSpeed;

        float wobbleX = Mathf.Sin(time) * wobbleAmount;
        float wobbleZ = Mathf.Cos(time * 1.2f) * wobbleAmount;

        transform.localPosition = originalPosition +
                                  new Vector3(wobbleX, 0f, wobbleZ);
    }
}