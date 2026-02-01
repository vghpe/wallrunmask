using StarterAssets;
using UnityEngine;

public class BoostRamp : ActivatedObjectParent
{
    public Transform Target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FirstPersonController playerController = other.gameObject.GetComponent<FirstPersonController>();


            playerController.BoostDirection = Target.position - transform.position;
            Debug.Log(playerController.BoostDirection);
            playerController.BoostMod += playerController.DashSpeed * 1;
        }
    }
}
