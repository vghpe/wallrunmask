using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCinematicWallRun : MonoBehaviour
{
    [SerializeField] private Transform player;
    private FirstPersonController firstPersonController;
    private void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("PlayerCapsule").transform;
        }
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
            Vector3 wallRunMove = transform.forward * firstPersonController.WallRunningSpeed * Time.deltaTime;

            firstPersonController._controller.Move(wallRunMove);

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Vector3 jumpDir = (transform.forward + Vector3.up * 0.6f).normalized;
                StartCoroutine(WallJump(jumpDir, firstPersonController.WallJumpForce, firstPersonController.WallJumpDuration));

            }
            yield return null;
        }
    }

    IEnumerator WallJump(Vector3 direction, float force, float duration)
    {
        float t = 0f;
        firstPersonController.WallRun = false;
        while (t < duration)
        {
            firstPersonController._controller.Move(
                direction * force * Time.deltaTime
            );
            t += Time.deltaTime;
            yield return null;
        }
    }

}
