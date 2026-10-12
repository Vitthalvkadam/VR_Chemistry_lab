using UnityEngine;

public class HologramFloat : MonoBehaviour
{
    [Header("Hover Dynamics")]
    public float floatSpeed = 2.0f;
    public float floatAmplitude = 0.025f;

    [Header("Tilt Dynamics")]
    public float tiltSpeed = 1.2f;
    public float tiltAmount = 1.2f;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private void Awake()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    private void OnEnable()
    {
        transform.localPosition = initialLocalPos;
        transform.localRotation = initialLocalRot;
    }

    private void Update()
    {
        float timeVal = Time.time;

        // Smooth sine wave vertical displacement
        float newY = initialLocalPos.y + (Mathf.Sin(timeVal * floatSpeed) * floatAmplitude);
        transform.localPosition = new Vector3(initialLocalPos.x, newY, initialLocalPos.z);

        // Holographic tilt oscillation
        float tiltX = Mathf.Sin(timeVal * tiltSpeed) * tiltAmount;
        float tiltZ = Mathf.Cos(timeVal * tiltSpeed * 0.7f) * tiltAmount;
        transform.localRotation = initialLocalRot * Quaternion.Euler(tiltX, 0f, tiltZ);
    }
}