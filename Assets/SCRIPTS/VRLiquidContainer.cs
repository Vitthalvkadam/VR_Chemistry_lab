using UnityEngine;
using System.Collections.Generic;

public class VRLiquidContainer : MonoBehaviour
{
    [Header("Chemical Properties")]
    public string chemicalID = "Acid";
    public Color chemicalColor = Color.green;

    [Header("Volume Settings")]
    public float currentVolume = 100f;
    public float maxVolume = 100f;
    public float pourRate = 20f; // Volume transferred per second

    [Header("Pour Detection")]
    [Range(0f, 180f)] public float pourThresholdAngle = 45f;
    public Transform pourPoint;
    public float detectionRadius = 0.25f;
    public LayerMask containerLayer;

    [Header("Visual References")]
    public Renderer liquidRenderer;
    public Transform liquidMeshTransform; // Cylinder mesh scaled on Y-axis (optional)
    public string fillPropertyName = "_FillAmount";
    public ParticleSystem pourParticles;
    public ParticleSystem smokeParticles;

    // Internal State
    private int fillPropertyID;
    private Material instanceMaterial;
    private HashSet<string> containedChemicals = new HashSet<string>();

    public bool IsPouring { get; private set; }

    private void Start()
    {
        fillPropertyID = Shader.PropertyToID(fillPropertyName);

        if (liquidRenderer != null)
        {
            instanceMaterial = liquidRenderer.material;
            instanceMaterial.color = chemicalColor;
        }

        if (currentVolume > 0f)
        {
            containedChemicals.Add(chemicalID);
        }

        if (smokeParticles != null) smokeParticles.Stop();

        UpdateVisuals();
    }

    private void Update()
    {
        // Check tilt angle relative to world UP
        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);
        IsPouring = (tiltAngle >= pourThresholdAngle) && (currentVolume > 0f);

        // Control visual particle stream state
        if (pourParticles != null)
        {
            var main = pourParticles.main;
            main.startColor = chemicalColor;

            if (IsPouring && !pourParticles.isPlaying) pourParticles.Play();
            else if (!IsPouring && pourParticles.isPlaying) pourParticles.Stop();
        }

        if (IsPouring)
        {
            TransferLiquid();
        }
    }

    private void TransferLiquid()
    {
        float amountToPour = pourRate * Time.deltaTime;
        amountToPour = Mathf.Min(amountToPour, currentVolume);

        if (amountToPour <= 0f) return;

        // Detect container underneath pour point
        if (pourPoint == null) pourPoint = transform;
        Collider[] hits = Physics.OverlapSphere(pourPoint.position, detectionRadius, containerLayer);

        VRLiquidContainer targetContainer = null;
        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                targetContainer = hit.GetComponentInParent<VRLiquidContainer>();
                if (targetContainer != null) break;
            }
        }

        // Check if liquid can transfer
        if (targetContainer != null && targetContainer.currentVolume < targetContainer.maxVolume)
        {
            currentVolume -= amountToPour;
            targetContainer.ReceiveLiquid(amountToPour, chemicalColor, chemicalID);
            UpdateVisuals();
        }
        else if (targetContainer == null)
        {
            // Drain container even if pouring onto floor/table
            currentVolume -= amountToPour;
            UpdateVisuals();
        }
    }

    public void ReceiveLiquid(float amount, Color incomingColor, string incomingID)
    {
        float newVolume = Mathf.Min(currentVolume + amount, maxVolume);

        // Blend colors proportionally
        if (currentVolume <= 0.01f)
        {
            chemicalColor = incomingColor;
        }
        else
        {
            float blendRatio = amount / newVolume;
            chemicalColor = Color.Lerp(chemicalColor, incomingColor, blendRatio);
        }

        currentVolume = newVolume;
        containedChemicals.Add(incomingID);

        UpdateVisuals();
        CheckReactions();
    }

    private void UpdateVisuals()
    {
        float fillRatio = Mathf.Clamp01(currentVolume / maxVolume);

        // Update Shader property if used
        if (instanceMaterial != null)
        {
            instanceMaterial.color = chemicalColor;
            if (instanceMaterial.HasProperty(fillPropertyID))
            {
                instanceMaterial.SetFloat(fillPropertyID, fillRatio);
            }
        }

        // Update local Y-scale for mesh fallback
        if (liquidMeshTransform != null)
        {
            Vector3 scale = liquidMeshTransform.localScale;
            liquidMeshTransform.localScale = new Vector3(scale.x, fillRatio, scale.z);
        }
    }

    private void CheckReactions()
    {
        // Example Reaction: Mixing Acid and Base
        if (containedChemicals.Contains("Acid") && containedChemicals.Contains("Base"))
        {
            TriggerReaction(Color.magenta, true);
        }
    }

    public void TriggerReaction(Color targetColor, bool enableSmoke)
    {
        chemicalColor = Color.Lerp(chemicalColor, targetColor, Time.deltaTime * 5f);
        UpdateVisuals();

        if (enableSmoke && smokeParticles != null && !smokeParticles.isPlaying)
        {
            smokeParticles.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pourPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pourPoint.position, detectionRadius);
        }
    }
}