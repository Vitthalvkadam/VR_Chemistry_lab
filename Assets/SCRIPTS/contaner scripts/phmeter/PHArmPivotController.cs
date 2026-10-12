using UnityEngine;

public class PHArmPivotController : MonoBehaviour
{
    [Header("Arm Rotation")]
    [Tooltip("Rotation angle around the X axis.")]
    [SerializeField] private float rotationX = 60f;

    [Tooltip("Should the arm rotate automatically when selected?")]
    [SerializeField] private bool rotateOnSelect = true;

    [Tooltip("Should the arm return to its original position when deselected?")]
    [SerializeField] private bool returnOnDeselect = false;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    private Quaternion originalRotation;
    private Quaternion targetRotation;

    // Track state (true = arm is raised up, false = arm is at original position)
    private bool isUp = false;

    private void Start()
    {
        // Remember the original ArmPivot rotation
        originalRotation = transform.localRotation;

        // Initial target
        targetRotation = originalRotation;
    }

    private void Update()
    {
        // Smoothly rotate toward target
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // =========================================
    // TOGGLE ARM (Click once = UP, Click again = DOWN)
    // =========================================

    public void ToggleArm()
    {
        if (isUp)
        {
            ReturnArm();
        }
        else
        {
            RotateArm();
        }
    }

    // =========================================
    // SELECT / GRAB
    // =========================================

    public void OnArmSelected()
    {
        Debug.Log("PH ARM SELECTED");

        if (!rotateOnSelect)
            return;

        // Toggles between raised and lowered position on every select/click
        ToggleArm();
    }

    // =========================================
    // DESELECT / RELEASE
    // =========================================

    public void OnArmDeselected()
    {
        Debug.Log("PH ARM DESELECTED");

        if (returnOnDeselect)
        {
            ReturnArm();
        }
    }

    // =========================================
    // ROTATE ARM
    // =========================================

    public void RotateArm()
    {
        targetRotation =
            originalRotation *
            Quaternion.Euler(rotationX, 0f, 0f);

        isUp = true;

        Debug.Log(
            "PH ARM ROTATING X: " +
            rotationX +
            " degrees"
        );
    }

    // =========================================
    // RETURN ARM
    // =========================================

    public void ReturnArm()
    {
        targetRotation = originalRotation;

        isUp = false;

        Debug.Log("PH ARM RETURNING TO ORIGINAL POSITION");
    }

    // =========================================
    // MANUAL SET ANGLE
    // =========================================

    public void SetRotationX(float angle)
    {
        rotationX = angle;

        RotateArm();
    }
}