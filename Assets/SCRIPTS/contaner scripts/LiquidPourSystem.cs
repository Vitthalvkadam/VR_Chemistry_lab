using UnityEngine;

public class LiquidPourSystem : MonoBehaviour
{
    [Header("Dependencies")]
    public LiquidWobbleContainer bottleContainer;
    public ParticleSystem pourParticleSystem;
    public Transform pourOrigin;

    [Header("Pour Settings")]
    public float pourAngleThreshold = 80.0f; // Degrees tilt needed to start pouring
    public float maxPourRate = 50.0f;

    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        if (pourParticleSystem != null)
        {
            emissionModule = pourParticleSystem.emission;
            emissionModule.rateOverTime = 0;
        }
    }

    void Update()
    {
        if (bottleContainer == null || pourParticleSystem == null) return;

        bool canPour = bottleContainer.isCapOpen && (bottleContainer.fillAmount > 0);
        float currentAngle = Vector3.Angle(transform.up, Vector3.up);

        if (canPour && currentAngle > pourAngleThreshold)
        {
            // Calculate pour strength based on angle tilt
            float tiltFactor = Mathf.InverseLerp(pourAngleThreshold, 180f, currentAngle);
            float currentRate = Mathf.Lerp(0, maxPourRate, tiltFactor);

            emissionModule.rateOverTime = currentRate;

            // Reduce internal liquid volume as it pours
            bottleContainer.fillAmount -= (currentRate * 0.001f) * Time.deltaTime;
        }
        else
        {
            emissionModule.rateOverTime = 0;
        }
    }
}