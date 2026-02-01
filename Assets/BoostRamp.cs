using StarterAssets;
using UnityEngine;

public class BoostRamp : ActivatedObjectParent
{
    public Transform Target;
    [Tooltip("If true, always boost in world +Z direction. If false, boost toward Target.")]
    public bool useWorldForward = true;
    
    [Tooltip("Multiplier for boost strength")]
    public float boostMultiplier = 5f;
    
    [Tooltip("Upward force to add (for ramp launch feel)")]
    public float upwardBoost = 8f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"BoostRamp: OnTriggerEnter with {other.name}, tag: {other.tag}");
        
        if (other.tag == "Player" && Activated)
        {
            FirstPersonController playerController = other.gameObject.GetComponent<FirstPersonController>();

            if (playerController == null)
            {
                Debug.LogError("BoostRamp: FirstPersonController component NOT FOUND on player!");
                return;
            }

            // Set boost direction in world space
            Vector3 boostDir;
            if (useWorldForward)
            {
                boostDir = Vector3.forward; // World +Z
            }
            else
            {
                boostDir = (Target.position - transform.position).normalized;
            }
            
            // Add upward component for ramp launch
            float boostPower = playerController.DashSpeed * boostMultiplier;
            boostDir.y = upwardBoost / boostPower;
            
            // Use TriggerBoost to properly start the boost
            playerController.TriggerBoost(boostPower, boostDir.normalized);
            
            Debug.Log($"BoostRamp: BOOST APPLIED! Direction: {boostDir.normalized}, BoostPower: {boostPower}, DashSpeed: {playerController.DashSpeed}");
        }
        else
        {
            Debug.Log($"BoostRamp: Ignoring non-Player object");
        }
    }
}
