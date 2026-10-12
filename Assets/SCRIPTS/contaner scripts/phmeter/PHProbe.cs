using UnityEngine;

public class PHProbe : MonoBehaviour
{
    [Header("Current Reading")]
    [SerializeField]
    private float currentPH = 7.0f;

    private Liquid currentLiquid;


    // =========================================
    // PUBLIC PROPERTIES
    // =========================================

    public float CurrentPH => currentPH;

    public bool IsInLiquid => currentLiquid != null;

    public bool IsInAir => currentLiquid == null;

    public Liquid CurrentLiquid => currentLiquid;


    // =========================================
    // LIQUID DETECTION
    // =========================================

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "PROBE TRIGGER ENTER: " +
            other.gameObject.name
        );

        Liquid liquid =
            other.GetComponentInParent<Liquid>();

        if (liquid == null)
        {
            Debug.Log(
                "Object does NOT contain Liquid component."
            );

            return;
        }

        currentLiquid = liquid;

        currentPH = liquid.GetPH();

        Debug.Log(
            "LIQUID DETECTED: " +
            liquid.LiquidName +
            " | pH = " +
            currentPH
        );
    }


    private void OnTriggerStay(Collider other)
    {
        Liquid liquid =
            other.GetComponentInParent<Liquid>();

        if (liquid == null)
            return;

        currentLiquid = liquid;

        currentPH = liquid.GetPH();
    }


    private void OnTriggerExit(Collider other)
    {
        Liquid liquid =
            other.GetComponentInParent<Liquid>();

        if (liquid == null)
            return;

        if (liquid == currentLiquid)
        {
            Debug.Log(
                "PROBE LEFT LIQUID: " +
                liquid.LiquidName
            );

            currentLiquid = null;

            // Probe is now in air
            currentPH = 7.0f;
        }
    }
}