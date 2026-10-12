using TMPro;
using UnityEngine;

public class PHDisplayController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PHProbe probe;
    [SerializeField] private TMP_Text phReadingDisplay;
    [SerializeField] private TMP_Text statusDisplay;

    [Header("Power")]
    [SerializeField] private bool isPoweredOn = false;


    private void Start()
    {
        // Keep display visible even when OFF
        if (statusDisplay != null)
            statusDisplay.gameObject.SetActive(true);

        if (phReadingDisplay != null)
            phReadingDisplay.gameObject.SetActive(true);

        UpdateDisplay();
    }


    private void Update()
    {
        UpdateDisplay();
    }


    private void UpdateDisplay()
    {
        // =========================================
        // POWER OFF
        // =========================================

        if (!isPoweredOn)
        {
            if (statusDisplay != null)
                statusDisplay.text = "POWER OFF";

            if (phReadingDisplay != null)
                phReadingDisplay.text = "pH: ---";

            return;
        }


        // =========================================
        // NO PROBE REFERENCE
        // =========================================

        if (probe == null)
        {
            if (statusDisplay != null)
                statusDisplay.text = "NO PROBE";

            if (phReadingDisplay != null)
                phReadingDisplay.text = "pH: ---";

            return;
        }


        // =========================================
        // PROBE IS IN LIQUID
        // =========================================

        if (probe.IsInLiquid)
        {
            if (statusDisplay != null)
                statusDisplay.text = "MEASURING";

            if (phReadingDisplay != null)
            {
                phReadingDisplay.text =
                    "pH: " + probe.CurrentPH.ToString("0.00");
            }

            return;
        }


        // =========================================
        // PROBE IS IN AIR
        // =========================================

        if (probe.IsInAir)
        {
            if (statusDisplay != null)
                statusDisplay.text = "IN AIR";

            if (phReadingDisplay != null)
                phReadingDisplay.text = "pH: ---";

            return;
        }
    }


    // =========================================
    // ON / OFF BUTTON
    // =========================================

    public void TogglePower()
    {
        isPoweredOn = !isPoweredOn;

        Debug.Log(
            "pH METER POWER: " +
            (isPoweredOn ? "ON" : "OFF")
        );

        UpdateDisplay();
    }


    public void TurnOn()
    {
        isPoweredOn = true;

        Debug.Log("pH METER POWER: ON");

        UpdateDisplay();
    }


    public void TurnOff()
    {
        isPoweredOn = false;

        Debug.Log("pH METER POWER: OFF");

        UpdateDisplay();
    }


    public bool IsPoweredOn()
    {
        return isPoweredOn;
    }
}