using System.Collections;
using UnityEngine;

public class CabinetDoorController : MonoBehaviour
{
    [Header("Door Rotation Settings")]
    [Tooltip("Angle to swing open relative to closed position (e.g. 110 for right door, -110 for left door)")]
    public float openAngle = 110f; 
    public float openSpeed = 3.0f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine doorCoroutine;

    private void Awake()
    {
        // Capture initial closed rotation automatically
        closedRotation = transform.localRotation;
        // Calculate open position outward along Y-axis relative to local starting angle
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        Quaternion target = isOpen ? openRotation : closedRotation;

        if (doorCoroutine != null) StopCoroutine(doorCoroutine);
        doorCoroutine = StartCoroutine(AnimateDoor(target));
    }

    private IEnumerator AnimateDoor(Quaternion target)
    {
        while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localRotation = target;
    }
}