using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class XRContainerInteraction : MonoBehaviour
{
    [Header("References")]
    public LiquidWobbleContainer wobbleScript;
    public GameObject bottleCapObject;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        // Subscribe to XR Grab events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        grabInteractable.activated.AddListener(OnTriggerPressed);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Hand is holding the bottle
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Bottle released from hand
    }

    // Triggered when user presses Trigger/Activate while holding the object
    private void OnTriggerPressed(ActivateEventArgs args)
    {
        // Toggle Cap Open/Closed on trigger press
        if (wobbleScript != null)
        {
            wobbleScript.isCapOpen = !wobbleScript.isCapOpen;

            if (bottleCapObject != null)
            {
                bottleCapObject.SetActive(!wobbleScript.isCapOpen);
            }
        }
    }
}