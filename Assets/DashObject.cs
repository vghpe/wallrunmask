using StarterAssets;
using UnityEngine;

public class DashObject : MonoBehaviour
{
    public GameObject ParentObj;
    public float ModThreshold = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FirstPersonController playerController = other.gameObject.GetComponent<FirstPersonController>();

            Debug.Log(playerController.DashMod);
            if (playerController.DashMod > ModThreshold)
            {
                DashBreak();

                playerController.DashMod += playerController.DashSpeed * 2;
            }
        }
    }

    public void DashBreak()
    {
        ParentObj.SetActive(false);
    }
}