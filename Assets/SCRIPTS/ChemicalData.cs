using UnityEngine;

/// <summary>
/// A reusable data definition for any chemical in your lab.
/// Create one asset per chemical: Assets > Create > Chemistry > Chemical Data
/// This is the core of the "data-driven" system — no hardcoded chemical logic,
/// just data. Adding chemical #21 later means creating one more asset, not writing more code.
/// </summary>
[CreateAssetMenu(fileName = "NewChemical", menuName = "Chemistry/Chemical Data")]
public class ChemicalData : ScriptableObject
{
    [Header("Identity")]
    public string chemicalName;       // e.g. "Hydrochloric Acid"
    public string formula;            // e.g. "HCl"

    [Header("Properties")]
    public float concentration;       // mol/L
    public float density = 1f;        // g/mL (used for volume/mass calcs if needed)
    public float pH = 7f;             // approximate pH of the pure solution
    public Color liquidColor = Color.white;

    [Header("Behaviour Flags")]
    public bool isIndicator = false;  // true for phenolphthalein, litmus, etc.
    public bool isAcid = false;
    public bool isBase = false;

    [Header("Hazard (optional, for UI/report)")]
    [TextArea] public string hazardNote;
}
