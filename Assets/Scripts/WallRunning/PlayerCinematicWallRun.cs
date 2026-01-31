using StarterAssets;
using System.Collections;
using UnityEngine;

public class PlayerCinematicWallRun : MonoBehaviour
{
    [SerializeField] private Transform player;
    private FirstPersonController firstPersonController;
    private void Start()
    {
        firstPersonController = player.GetComponent<FirstPersonController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        firstPersonController.WallRun = true;
        StartCoroutine(TakeControl());
    }
    private void OnTriggerExit(Collider other)
    {
        firstPersonController.WallRun = false;
    }
    IEnumerator TakeControl()
    {
        while (firstPersonController.WallRun)
        {
            player.Translate(Vector3.forward * firstPersonController.WallRunningSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
