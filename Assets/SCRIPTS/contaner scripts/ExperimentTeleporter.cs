using UnityEngine;

public class ExperimentTeleporter : MonoBehaviour
{
    [Header("XR Rig Reference")]
    public Transform xrOrigin; // Drag your XR Origin object here

    [Header("Experiment Anchors (Index 0 = Exp 1, Index 1 = Exp 2, etc.)")]
    public Transform[] experimentAnchors; // Array of 8 anchors

    private int selectedExperimentIndex = 0; // Currently selected experiment (0 to 7)

    /// <summary>
    /// Call this from your Experiment Selection Buttons (Exp 1 - Exp 8)
    /// </summary>
    public void SetSelectedExperiment(int experimentNumber)
    {
        // Convert 1-based index (1-8) to 0-based array index (0-7)
        selectedExperimentIndex = experimentNumber - 1;
    }

    /// <summary>
    /// Call this when the 'Enter Lab Button' is clicked
    /// </summary>
    public void TeleportToSelectedLab()
    {
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin is not assigned in the Teleporter script!");
            return;
        }

        if (experimentAnchors == null || experimentAnchors.Length == 0)
        {
            Debug.LogError("Experiment Anchors are not assigned!");
            return;
        }

        if (selectedExperimentIndex >= 0 && selectedExperimentIndex < experimentAnchors.Length)
        {
            Transform targetAnchor = experimentAnchors[selectedExperimentIndex];

            if (targetAnchor != null)
            {
                // Teleport position and orientation
                xrOrigin.SetPositionAndRotation(targetAnchor.position, targetAnchor.rotation);
                Debug.Log($"Teleported to Experiment {selectedExperimentIndex + 1}");
            }
        }
    }
}