using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to your Flask/Beaker GameObject.
/// It tracks every chemical currently inside the container and how much (in mL),
/// updates the visible liquid level + mixed color, and tells the ReactionManager
/// whenever new liquid is added so it can check for a reaction.
/// </summary>
public class ChemicalContainer : MonoBehaviour
{
    [Header("Contents (runtime)")]
    // key = chemical name, value = volume in mL currently in the flask
    public Dictionary<ChemicalData, float> contents = new Dictionary<ChemicalData, float>();

    [Header("Visuals")]
    public Transform liquidVisual;      // a scaled cylinder/mesh sitting inside the flask
    public Renderer liquidRenderer;     // its renderer, to change color
    public float maxVisualHeight = 0.2f; // local scale.y when the flask is "full"
    public float maxCapacityML = 250f;   // treat this as a 250 mL flask

    [Header("Events")]
    public ReactionManager reactionManager;

    private float TotalVolume()
    {
        float total = 0f;
        foreach (var kvp in contents) total += kvp.Value;
        return total;
    }

    /// <summary>
    /// Called by PourController when a stream of chemical enters this container's trigger.
    /// </summary>
    public void AddChemical(ChemicalData chem, float amountML)
    {
        if (chem == null || amountML <= 0f) return;

        if (contents.ContainsKey(chem))
            contents[chem] += amountML;
        else
            contents[chem] = amountML;

        UpdateVisuals();

        if (reactionManager != null)
            reactionManager.CheckReaction(this);
    }

    private void UpdateVisuals()
    {
        float total = TotalVolume();
        float fillRatio = Mathf.Clamp01(total / maxCapacityML);

        if (liquidVisual != null)
        {
            Vector3 scale = liquidVisual.localScale;
            scale.y = fillRatio * maxVisualHeight;
            liquidVisual.localScale = scale;
        }

        if (liquidRenderer != null)
            liquidRenderer.material.color = GetMixedColor();
    }

    /// <summary>
    /// Simple volume-weighted average of all liquid colors currently mixed together.
    /// Good enough for a convincing visual without simulating real chemistry of color mixing.
    /// </summary>
    public Color GetMixedColor()
    {
        float total = TotalVolume();
        if (total <= 0f) return new Color(1, 1, 1, 0.15f); // empty = near-invisible

        Color mixed = Color.black;
        foreach (var kvp in contents)
        {
            float weight = kvp.Value / total;
            mixed += kvp.Key.liquidColor * weight;
        }
        mixed.a = 0.85f; // keep it slightly translucent, looks like liquid
        return mixed;
    }

    public float GetAmount(ChemicalData chem)
    {
        return contents.ContainsKey(chem) ? contents[chem] : 0f;
    }

    public void ClearContainer()
    {
        contents.Clear();
        UpdateVisuals();
    }
}
