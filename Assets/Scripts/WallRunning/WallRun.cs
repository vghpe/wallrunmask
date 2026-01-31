using StarterAssets;
using System.Collections;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] private FirstPersonController firstPersonController;
    private float playerGravity;
    private float playerJump;
    private void OnTriggerEnter(Collider other)
    {
        playerGravity = firstPersonController.Gravity;
        playerJump = firstPersonController.JumpHeight;
        firstPersonController.JumpHeight = 0.0f;
        firstPersonController.Gravity = 0.0f;
        StartCoroutine(TakeControl());
        Debug.Log("Entered wall running.");
    }

    private void OnTriggerExit(Collider other)
    {
        firstPersonController.Gravity = playerGravity;
        firstPersonController.JumpHeight = playerJump;
        StopAllCoroutines();
        Debug.Log("Stopped wall running.");
    }

    IEnumerator TakeControl()
    {
        while (!firstPersonController.WallRun)
        {
            Vector3 toTarget = transform.position - firstPersonController.transform.position;
            Vector3 pull = toTarget.normalized * firstPersonController.ControlFactorDragAgainstWall * Time.deltaTime;
            firstPersonController._controller.Move(pull);
            yield return null;
        }
    }
}
