using StarterAssets;
using UnityEngine;

public class BoostRamp : ActivatedObjectParent
{
    public Transform Target;
    public float BoostMagnitude = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FirstPersonController playerController = other.gameObject.GetComponent<FirstPersonController>();

            playerController.BoostDirection = Target.position - transform.position;
            playerController.BoostMod += playerController.DashSpeed * BoostMagnitude;
        }
    }
}
