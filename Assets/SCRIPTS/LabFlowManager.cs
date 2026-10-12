using UnityEngine;
using TMPro;

public class LabFlowManager : MonoBehaviour
{
    [System.Serializable]
    public struct ExperimentData
    {
        public int experimentID;
        public string experimentTitle;
        [TextArea(3, 5)]
        public string requiredEquipment;
        [TextArea(2, 4)]
        public string safetyNote;
    }

    [Header("UI Panels")]
    [Tooltip("Drag WelcomePanel here")]
    public GameObject welcomePanel;
    [Tooltip("Drag ExperimentSelectionPanel here")]
    public GameObject selectionPanel;
    [Tooltip("Drag EquipmentInfoPanel here")]
    public GameObject equipmentInfoPanel;

    [Header("Equipment Info UI References")]
    public TMP_Text equipmentTitleText;
    public TMP_Text equipmentDescriptionText;
    public TMP_Text safetyNoteText;

    [Header("Experiment Database (8 Experiments)")]
    public ExperimentData[] experiments = new ExperimentData[8];

    private int currentSelectedID = 1;

    private void Awake()
    {
        InitializeDefaultExperimentData();
    }

    private void Start()
    {
        // Start game at Welcome Screen
        ShowWelcomeScreen();
    }

    // --- Screen Transitions ---

    public void ShowWelcomeScreen()
    {
        SetPanelStates(welcome: true, selection: false, info: false);
    }

    public void ShowSelectionScreen()
    {
        SetPanelStates(welcome: false, selection: true, info: false);
    }

    public void SelectExperiment(int experimentID)
    {
        currentSelectedID = Mathf.Clamp(experimentID, 1, 8);
        UpdateEquipmentDisplay(currentSelectedID);
        SetPanelStates(welcome: false, selection: false, info: true);
    }

    public void EnterLab()
    {
        // Hide UI and begin experiment
        SetPanelStates(welcome: false, selection: false, info: false);
        Debug.Log($"[LabFlowManager] Entering Lab for Experiment #{currentSelectedID}");

        // TODO: Add your Lab spawner or XR player teleportation code here
    }

    // --- Helper Methods ---

    private void SetPanelStates(bool welcome, bool selection, bool info)
    {
        if (welcomePanel != null) welcomePanel.SetActive(welcome);
        if (selectionPanel != null) selectionPanel.SetActive(selection);
        if (equipmentInfoPanel != null) equipmentInfoPanel.SetActive(info);
    }

    private void UpdateEquipmentDisplay(int id)
    {
        int index = id - 1;
        if (index >= 0 && index < experiments.Length)
        {
            if (equipmentTitleText != null)
                equipmentTitleText.text = experiments[index].experimentTitle;

            if (equipmentDescriptionText != null)
                equipmentDescriptionText.text = experiments[index].requiredEquipment;

            if (safetyNoteText != null)
                safetyNoteText.text = "Safety Note: " + experiments[index].safetyNote;
        }
    }

    private void InitializeDefaultExperimentData()
    {
        string[] titles = {
            "Exp 1: Acid-Base Neutralization",
            "Exp 2: Thermal Decomposition",
            "Exp 3: Titration Analysis",
            "Exp 4: Precipitation Reaction",
            "Exp 5: Redox Titration",
            "Exp 6: Gas Law Verification",
            "Exp 7: Chemical Kinetics",
            "Exp 8: Electrolysis Setup"
        };

        string[] equipment = {
            "• HCl Beaker\n• NaOH Beaker\n• pH Indicator Strip",
            "• Test Tubes & Rack\n• Bunsen Burner\n• Tongs",
            "• Buret & Stand\n• Erlenmeyer Flask\n• Phenolphthalein",
            "• Silver Nitrate\n• Sodium Chloride\n• Beaker & Stirrer",
            "• Potassium Permanganate\n• Volumetric Flask\n• Pipette",
            "• Gas Syringe\n• Sealed Flask\n• Pressure Sensor",
            "• Stopwatch\n• Reactant Solutions\n• Colorimeter",
            "• Electrodes (Cu/Zn)\n• DC Power Supply\n• Electrolyte Bath"
        };

        string[] safety = {
            "Wear gloves when handling strong acids.",
            "Use tongs; glass will remain hot after flame exposure.",
            "Ensure buret valve is closed before pouring liquids.",
            "Avoid direct skin contact with silver nitrate.",
            "Potassium permanganate can stain surfaces and skin.",
            "Do not over-pressurize glass apparatus.",
            "Record timer immediately upon mixing solutions.",
            "Turn off DC power supply before removing electrodes."
        };

        for (int i = 0; i < 8; i++)
        {
            experiments[i].experimentID = i + 1;
            experiments[i].experimentTitle = titles[i];
            experiments[i].requiredEquipment = equipment[i];
            experiments[i].safetyNote = safety[i];
        }
    }
}