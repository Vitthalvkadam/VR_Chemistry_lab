using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LiquidWobbleContainer : MonoBehaviour
{
    [Header("Bottle & Cap Settings")]
    public Transform bottleTransform;
    public bool isCapOpen = false;
    public Transform capOpeningPoint; // Near the top mouth of the bottle

    [Header("Wobble Properties")]
    public float maxWobble = 0.03f;
    public float wobbleSpeed = 1.0f;
    public float recovery = 2.0f;

    [Header("Fill Properties")]
    [Range(0f, 1f)] public float fillAmount = 0.5f;

    private Vector3 lastPos;
    private Vector3 velocity;
    private Vector3 lastRot;
    private Vector3 angularVelocity;

    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float pulse;
    private float time;

    private Renderer rendererComponent;

    void Start()
    {
        rendererComponent = GetComponent<Renderer>();
        if (bottleTransform == null) bottleTransform = transform.parent;
        lastPos = bottleTransform.position;
        lastRot = bottleTransform.rotation.eulerAngles;
    }

    void Update()
    {
        time += Time.deltaTime;

        // Calculate wobble decay
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * recovery);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * recovery);

        // Calculate continuous sine wave for wobble
        pulse = 2 * Mathf.PI * wobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Cos(pulse * time);

        // Calculate accelerations from bottle motion
        velocity = (bottleTransform.position - lastPos) / Time.deltaTime;
        angularVelocity = (bottleTransform.rotation.eulerAngles - lastRot) / Time.deltaTime;

        // Apply forces to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * maxWobble, -maxWobble, maxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * maxWobble, -maxWobble, maxWobble);

        lastPos = bottleTransform.position;
        lastRot = bottleTransform.rotation.eulerAngles;

        // If cap is open and bottle is violently shaken up/down, spill liquid out
        if (isCapOpen && velocity.y < -3.0f && fillAmount > 0)
        {
            fillAmount -= Time.deltaTime * 0.1f;
        }

        // Send parameters to the material shader properties dynamically
        rendererComponent.material.SetFloat("_WobbleX", wobbleAmountX);
        rendererComponent.material.SetFloat("_WobbleZ", wobbleAmountZ);
        rendererComponent.material.SetFloat("_FillAmount", fillAmount);
    }
}