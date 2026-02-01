using StarterAssets;
using UnityEngine;

public class BoostRamp : ActivatedObjectParent
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FirstPersonController playerController = other.gameObject.GetComponent<FirstPersonController>();

            playerController.BoostDirection = new Vector3(0,0.5f,1);
            Debug.Log(playerController.BoostDirection);
            playerController.BoostMod += playerController.DashSpeed * 6;
        }
    }
}
