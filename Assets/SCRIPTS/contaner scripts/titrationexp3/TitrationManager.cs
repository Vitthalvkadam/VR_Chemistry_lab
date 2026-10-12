using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitrationManager : MonoBehaviour
{
    [Header("Titration Setup Parameters")]
    public float sampleVolumemL = 20.0f; // Initial acid volume in flask
    public float acidConcentration = 0.1f;
    public float baseConcentration = 0.1f;
    public float maxBuretteCapacitymL = 50.0f;

    [Header("Dynamic State")]
    public float currentBaseAddedmL = 0.0f;
    public float currentpH = 1.0f;

    [Header("Unity Visual References")]
    public ParticleSystem liquidStream;
    public Transform stopcockPivot;
    public Renderer flaskLiquidRenderer;
    public Renderer buretteLiquidRenderer;

    [Header("Liquid Levels (Transforms for Scaling)")]
    public Transform flaskLiquidTransform;
    public Transform buretteLiquidTransform;

    [Header("UI References")]
    public TextMeshProUGUI phDisplayText;
    public TextMeshProUGUI volumeDisplayText;
    public TextMeshProUGUI statusText;

    [Header("Color Map (Phenolphthalein)")]
    public Color acidicColor = new Color(1f, 1f, 1f, 0.15f);     // Clear/Transparent
    public Color endpointColor = new Color(1f, 0.41f, 0.71f, 0.6f); // Pale Pink
    public Color basicColor = new Color(0.85f, 0.1f, 0.5f, 0.85f);  // Magenta

    private bool isTitrating = false;
    private Vector3 initialFlaskScale;
    private Vector3 initialBuretteScale;

    void Start()
    {
        if (flaskLiquidTransform != null) initialFlaskScale = flaskLiquidTransform.localScale;
        if (buretteLiquidTransform != null) initialBuretteScale = buretteLiquidTransform.localScale;

        UpdateTitrationState();
    }

    public void AddBaseDrop(float volumeIncrementmL)
    {
        if (isTitrating) return;
        StartCoroutine(AnimateDropAddition(volumeIncrementmL));
    }

    private IEnumerator AnimateDropAddition(float volumeIncrementmL)
    {
        isTitrating = true;

        if (stopcockPivot != null)
            stopcockPivot.Rotate(0, 0, 45f);

        if (liquidStream != null)
            liquidStream.Play();

        yield return new WaitForSeconds(0.4f);

        if (liquidStream != null)
            liquidStream.Stop();

        if (stopcockPivot != null)
            stopcockPivot.Rotate(0, 0, -45f);

        currentBaseAddedmL += volumeIncrementmL;
        CalculatepH();
        UpdateTitrationState();

        isTitrating = false;
    }

    private void CalculatepH()
    {
        float totalVolumeL = (sampleVolumemL + currentBaseAddedmL) / 1000.0f;
        float initialAcidMoles = (sampleVolumemL / 1000.0f) * acidConcentration;
        float addedBaseMoles = (currentBaseAddedmL / 1000.0f) * baseConcentration;

        if (addedBaseMoles < initialAcidMoles)
        {
            float remainingAcidMoles = initialAcidMoles - addedBaseMoles;
            float hPlusConc = remainingAcidMoles / totalVolumeL;
            currentpH = -Mathf.Log10(hPlusConc);
        }
        else if (Mathf.Approximately(addedBaseMoles, initialAcidMoles))
        {
            currentpH = 7.00f;
        }
        else
        {
            float excessBaseMoles = addedBaseMoles - initialAcidMoles;
            float ohMinusConc = excessBaseMoles / totalVolumeL;
            float pOH = -Mathf.Log10(ohMinusConc);
            currentpH = 14.0f - pOH;
        }

        currentpH = Mathf.Clamp(currentpH, 0.0f, 14.0f);
    }

    private void UpdateTitrationState()
    {
        // 1. UI Text Updates
        if (phDisplayText != null) phDisplayText.text = $"pH: {currentpH:F2}";
        if (volumeDisplayText != null) volumeDisplayText.text = $"NaOH Added: {currentBaseAddedmL:F1} mL";

        // 2. Dynamic Liquid Scaling (Y Axis height change)
        if (flaskLiquidTransform != null)
        {
            float flaskScaleMultiplier = 1.0f + (currentBaseAddedmL / sampleVolumemL) * 0.4f;
            flaskLiquidTransform.localScale = new Vector3(initialFlaskScale.x, initialFlaskScale.y * flaskScaleMultiplier, initialFlaskScale.z);
        }

        if (buretteLiquidTransform != null)
        {
            float remainingBurettePercent = Mathf.Clamp01(1.0f - (currentBaseAddedmL / maxBuretteCapacitymL));
            buretteLiquidTransform.localScale = new Vector3(initialBuretteScale.x, initialBuretteScale.y * remainingBurettePercent, initialBuretteScale.z);
        }

        // 3. Smooth Color Tinting on Standard URP Material
        Color currentColor;
        if (currentpH < 8.2f)
        {
            currentColor = Color.Lerp(acidicColor, endpointColor, Mathf.InverseLerp(1.0f, 8.2f, currentpH));
        }
        else
        {
            currentColor = Color.Lerp(endpointColor, basicColor, Mathf.InverseLerp(8.2f, 12.0f, currentpH));
        }

        if (flaskLiquidRenderer != null)
        {
            flaskLiquidRenderer.material.color = currentColor;
        }

        // 4. Status Indicator
        if (statusText != null)
        {
            if (currentpH < 7.0f) statusText.text = "Status: Acidic Solution";
            else if (currentpH >= 7.0f && currentpH <= 9.0f) statusText.text = "Status: Endpoint Reached!";
            else statusText.text = "Status: Basic Solution (Over-titrated)";
        }
    }

    // --- Reset Function for UI Reset Button ---
    public void ResetExperiment()
    {
        StopAllCoroutines();
        isTitrating = false;
        currentBaseAddedmL = 0.0f;
        currentpH = 1.0f;

        if (flaskLiquidTransform != null) flaskLiquidTransform.localScale = initialFlaskScale;
        if (buretteLiquidTransform != null) buretteLiquidTransform.localScale = initialBuretteScale;

        if (liquidStream != null) liquidStream.Stop();

        UpdateTitrationState();
    }
}