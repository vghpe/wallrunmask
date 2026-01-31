using UnityEngine;

public class Killzone : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("The transform where the player will respawn")]
    public Transform respawnPoint;
    
    [Header("Particle System")]
    [Tooltip("Optional particle system to restart on respawn")]
    public ParticleSystem particleSystemToRestart;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            RespawnPlayer(other.gameObject);
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        if (respawnPoint != null)
        {
            // Get the CharacterController component if it exists (to disable/enable it)
            CharacterController characterController = player.GetComponent<CharacterController>();
            
            if (characterController != null)
            {
                // Temporarily disable CharacterController to allow position change
                characterController.enabled = false;
                player.transform.position = respawnPoint.position;
                player.transform.rotation = respawnPoint.rotation;
                characterController.enabled = true;
            }
            else
            {
                // If no CharacterController, just set position and rotation directly
                player.transform.position = respawnPoint.position;
                player.transform.rotation = respawnPoint.rotation;
            }

            // Reset velocity if the player has a Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            // Restart particle system if assigned
            if (particleSystemToRestart != null)
            {
                particleSystemToRestart.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystemToRestart.Play();
            }
        }
        else
        {
            Debug.LogWarning("Respawn point not set on Killzone!");
        }
    }
}
