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
        GameManager.Singleton.ColorChangeEvent.AddListener(ChangedMask);
    }

    void ChangedMask()
    {
        firstPersonController.WallRun = false;
        firstPersonController.WallSide = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        firstPersonController.WallRun = true;
        StartCoroutine(TakeControl());
    }
    private void OnTriggerExit(Collider other)
    {
        firstPersonController.WallRun = false;
        firstPersonController.WallSide = 0;
    }
    IEnumerator TakeControl()
    {
        Vector3 toWall = transform.position - firstPersonController.transform.position;
        firstPersonController.WallSide = Vector3.Dot(toWall, firstPersonController.transform.right) > 0 ? 1 : -1;
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
