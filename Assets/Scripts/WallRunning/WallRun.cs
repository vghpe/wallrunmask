using StarterAssets;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] private FirstPersonController firstPersonController;
    private float playerGravity;
    private float playerJump;
    private float stepOffset;
    private void Start()
    {
        if (firstPersonController == null)
        {
            firstPersonController = GameObject.Find("PlayerCapsule").GetComponent<FirstPersonController>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        playerGravity = firstPersonController.Gravity;
        playerJump = firstPersonController.JumpHeight;
        stepOffset = firstPersonController._controller.stepOffset;
        firstPersonController._controller.stepOffset = 0.0f;
        firstPersonController.JumpHeight = 0.0f;
        firstPersonController.Gravity = 0.0f;
        StartCoroutine(TakeControl());
        Debug.Log("Entered wall running.");
    }

    private void OnTriggerExit(Collider other)
    {
        firstPersonController.Gravity = playerGravity;
        firstPersonController.JumpHeight = playerJump;
        firstPersonController._controller.stepOffset = stepOffset;
        StopAllCoroutines(); //Probably not needed, since TakeControl should already have ended
        Debug.Log("Stopped wall running.");
    }

    IEnumerator TakeControl()
    {
        while (!firstPersonController.WallRun)
        {
            Vector3 toTarget = transform.position - firstPersonController.transform.position; 
            Vector3 pull = firstPersonController.WallDragPower * Time.deltaTime * toTarget.normalized; 
            firstPersonController._controller.Move(pull); 
            yield return null;
        }
    }
}
