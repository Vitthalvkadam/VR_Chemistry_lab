using UnityEngine;

public enum ToolType { Stirrer, Heater }

public class LabTool : MonoBehaviour
{
    public ToolType toolType;
    public Color reactionColor = Color.red;

    private void OnTriggerStay(Collider other)
    {
        VRLiquidContainer container = other.GetComponentInParent<VRLiquidContainer>();
        if (container != null && container.currentVolume > 0f)
        {
            if (toolType == ToolType.Heater)
            {
                container.TriggerReaction(Color.red, true); // Turns red & steams
            }
            else if (toolType == ToolType.Stirrer)
            {
                container.TriggerReaction(Color.cyan, false); // Blends into uniform color
            }
        }
    }
}