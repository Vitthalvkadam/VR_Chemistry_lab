using UnityEngine;

public class BeakerReceiver : MonoBehaviour
{
    [Header("Beaker Fill Properties")]
    public Transform liquidLevelMesh;
    [Range(0f, 1f)] public float currentLevel = 0.0f;
    public float fillSpeed = 0.005f;

    void OnParticleCollision(GameObject other)
    {
        // Add liquid volume when pour particles hit the beaker interior
        if (currentLevel < 1.0f)
        {
            currentLevel += fillSpeed;
            if (liquidLevelMesh != null)
            {
                liquidLevelMesh.localScale = new Vector3(
                    liquidLevelMesh.localScale.x, 
                    currentLevel, 
                    liquidLevelMesh.localScale.z
                );
            }
        }
    }
}