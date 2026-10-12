using UnityEngine;

/// <summary>
/// Attach this once per experiment scene (e.g. on an empty "ReactionManager" GameObject).
/// Assign references to your four ChemicalData assets: HCl, NaOH, Water, Phenolphthalein.
///
/// This script implements ONE reaction (acid-base neutralization with indicator) as a
/// concrete example. For experiments 2-5, you'd add more "Check_X" style rule methods
/// following the same pattern — the architecture already supports it.
/// </summary>
public class ReactionManager : MonoBehaviour
{
    [Header("Chemical References")]
    public ChemicalData hcl;
    public ChemicalData naoh;
    public ChemicalData water;
    public ChemicalData phenolphthalein;

    [Header("Effects")]
    public ParticleSystem neutralizationBurst;   // small fizz/sparkle effect on reaction
    public AudioSource reactionSound;

    [Header("Reaction Tuning")]
    public float molesPerML_HCl = 0.1f;   // matches ChemicalData.concentration convention
    public float molesPerML_NaOH = 0.1f;

    // Live experiment data, useful for the PDF report
    [System.NonSerialized] public float lastPH = 7f;
    [System.NonSerialized] public float lastTemperatureC = 25f;
    [System.NonSerialized] public string lastResultSummary = "";

    /// <summary>
    /// Called by ChemicalContainer every time new liquid is added.
    /// Recomputes the current state of the mixture.
    /// </summary>
    public void CheckReaction(ChemicalContainer flask)
    {
        float hclML = flask.GetAmount(hcl);
        float naohML = flask.GetAmount(naoh);
        float indicatorML = flask.GetAmount(phenolphthalein);

        // Moles present, using concentration * volume(L)
        float hclMoles = hcl != null ? hcl.concentration * (hclML / 1000f) : 0f;
        float naohMoles = naoh != null ? naoh.concentration * (naohML / 1000f) : 0f;

        float excessMoles = naohMoles - hclMoles; // positive = basic, negative = acidic, ~0 = neutral

        // --- Simplified pH estimate (good enough for a student demo, not lab-grade) ---
        if (Mathf.Abs(excessMoles) < 0.0005f)
        {
            lastPH = 7f;
            lastResultSummary = "Neutralized: HCl + NaOH -> NaCl + H2O";
            TriggerNeutralizationEffect();
        }
        else if (excessMoles > 0f)
        {
            lastPH = Mathf.Clamp(7f + excessMoles * 200f, 7f, 13f);
            lastResultSummary = "Solution is basic (excess NaOH)";
        }
        else
        {
            lastPH = Mathf.Clamp(7f + excessMoles * 200f, 1f, 7f);
            lastResultSummary = "Solution is acidic (excess HCl)";
        }

        // --- Indicator color response ---
        // Phenolphthalein: pink above pH ~8.2, colorless below.
        if (indicatorML > 0f)
        {
            bool shouldBePink = lastPH > 8.2f;
            Color targetTint = shouldBePink ? new Color(1f, 0.55f, 0.8f, 0.85f) : new Color(1f, 1f, 1f, 0.15f);
            if (flask.liquidRenderer != null)
                flask.liquidRenderer.material.color = targetTint;
        }

        // --- Simple exothermic temperature bump on neutralization ---
        if (lastResultSummary.StartsWith("Neutralized"))
        {
            lastTemperatureC = 25f + Mathf.Min(hclMoles, naohMoles) * 400f; // rough, tunable
        }
        else
        {
            lastTemperatureC = 25f;
        }
    }

    private void TriggerNeutralizationEffect()
    {
        if (neutralizationBurst != null) neutralizationBurst.Play();
        if (reactionSound != null) reactionSound.Play();
    }
}
