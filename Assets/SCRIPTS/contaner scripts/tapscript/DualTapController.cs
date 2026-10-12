using UnityEngine;

public class DualTapController : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem coldWaterParticles;
    [SerializeField] private ParticleSystem hotWaterParticles;

    private bool isColdWaterOn = false;
    private bool isHotWaterOn = false;

    private void Start()
    {
        // Ensure both streams are stopped on start
        if (coldWaterParticles != null && coldWaterParticles.isPlaying)
            coldWaterParticles.Stop();

        if (hotWaterParticles != null && hotWaterParticles.isPlaying)
            hotWaterParticles.Stop();
    }

    /// <summary>
    /// Call this from the Cold Water UI Button OnClick() event
    /// </summary>
    public void ToggleColdWater()
    {
        if (coldWaterParticles == null) return;

        isColdWaterOn = !isColdWaterOn;

        if (isColdWaterOn)
            coldWaterParticles.Play();
        else
            coldWaterParticles.Stop();
    }

    /// <summary>
    /// Call this from the Hot Water UI Button OnClick() event
    /// </summary>
    public void ToggleHotWater()
    {
        if (hotWaterParticles == null) return;

        isHotWaterOn = !isHotWaterOn;

        if (isHotWaterOn)
            hotWaterParticles.Play();
        else
            hotWaterParticles.Stop();
    }
}