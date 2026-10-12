using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to an empty "ExperimentController" GameObject in the scene.
/// Hook the OnFinishExperimentButtonPressed() method to a world-space UI Button
/// (XR-interactable canvas button) that the student presses to end the experiment
/// and generate the PDF report. This is the "glue" script for the demo.
/// </summary>
public class ExperimentController : MonoBehaviour
{
    public string experimentName = "Acid-Base Neutralization";
    public ReactionManager reactionManager;
    public ChemicalContainer flask;

    [Header("UI (optional, for on-screen confirmation)")]
    public Text statusText; // assign a world-space UI Text/TMP element

    public void OnFinishExperimentButtonPressed()
    {
        string path = PDFReportGenerator.GenerateReport(experimentName, reactionManager, flask);

        if (statusText != null)
            statusText.text = "Report saved!\n" + path;
    }
}
