using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LiquidWobble : MonoBehaviour
{
    [Header("Wobble Settings")]
    [SerializeField] private float maxWobble = 0.03f;
    [SerializeField] private float wobbleSpeed = 10f;
    [SerializeField] private float recoverySpeed = 2f; // Decays over ~2 seconds

    [Header("Sensitivity")]
    [SerializeField] private float movementSensitivity = 1f;
    [SerializeField] private float rotationSensitivity = 1f;

    private Renderer meshRenderer;
    private Vector3 lastPosition;
    private Vector3 lastRotation;

    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float pulse;

    // Shader Property IDs for optimized performance
    private static readonly int RotXID = Shader.PropertyToID("_rotx");
    private static readonly int RotZID = Shader.PropertyToID("_rotz");

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f) return;

        // 1. Decay existing wobble over time using Exponential Decay
        float decayFactor = Mathf.Exp(-recoverySpeed * deltaTime);
        wobbleAmountToAddX *= decayFactor;
        wobbleAmountToAddZ *= decayFactor;

        // 2. Calculate Sine Wave Oscillation
        pulse += deltaTime * wobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse);

        // 3. Send values to the material shader
        meshRenderer.material.SetFloat(RotXID, wobbleAmountX);
        meshRenderer.material.SetFloat(RotZID, wobbleAmountZ);

        // 4. Calculate Linear Velocity (Movement) in Local Space
        Vector3 velocity = (lastPosition - transform.position) / deltaTime;
        Vector3 angularVelocity = GetAngularVelocity();

        // 5. Accumulate new wobble from movement and rotation
        wobbleAmountToAddX += Mathf.Clamp((velocity.x * movementSensitivity + angularVelocity.z * rotationSensitivity) * maxWobble, -maxWobble, maxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z * movementSensitivity + angularVelocity.x * rotationSensitivity) * maxWobble, -maxWobble, maxWobble);

        // Update tracking variables for next frame
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }

    private Vector3 GetAngularVelocity()
    {
        Vector3 currentRotation = transform.eulerAngles;
        Vector3 deltaRotation = new Vector3(
            Mathf.DeltaAngle(lastRotation.x, currentRotation.x),
            Mathf.DeltaAngle(lastRotation.y, currentRotation.y),
            Mathf.DeltaAngle(lastRotation.z, currentRotation.z)
        );

        return deltaRotation / Time.deltaTime;
    }
}