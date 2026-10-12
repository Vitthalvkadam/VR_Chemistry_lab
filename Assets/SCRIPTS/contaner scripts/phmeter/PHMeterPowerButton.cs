using UnityEngine;

public class PHMeterPowerButton : MonoBehaviour
{
    [SerializeField] private PHDisplayController displayController;

    public void PressButton()
    {
        if (displayController == null)
        {
            Debug.LogError(
                "PHMeterPowerButton: Display Controller is not assigned!"
            );
            return;
        }

        displayController.TogglePower();
    }
}
