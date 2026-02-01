using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Killzone : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("The transform where the player will respawn")]
    public Transform respawnPoint;
    
    [Header("Quick Reset")]
    [Tooltip("Key to quickly reset the game")]
    public Key resetKey = Key.R;
    
    [Header("Particle System")]
    [Tooltip("Optional particle system to restart on respawn")]
    public ParticleSystem particleSystemToRestart;
    
    [Header("Music")]
    [Tooltip("Restart music tracks on respawn")]
    public bool restartMusicOnRespawn = false;
    
    [Header("Countdown")]
    [Tooltip("Show countdown on respawn")]
    public bool showCountdownOnRespawn = true;
    
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current[resetKey].wasPressedThisFrame)
        {
            QuickReset();
        }
#endif
    }

    public void QuickReset()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (_player != null)
        {
            RestartGame(_player);
        }
        else
        {
            Debug.LogWarning("Killzone: Player not found for quick reset!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            RestartGame(other.gameObject);
        }
    }

    public void RestartGame(GameObject other)
    {
        RespawnPlayer(other.gameObject);
        GameManager.Singleton.RestartGame();
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
            
            // Restart music if enabled
            if (restartMusicOnRespawn && MusicManager.Singleton != null)
            {
                MusicManager.Singleton.RestartAllTracks();
            }
            
            // Show countdown on respawn
            if (showCountdownOnRespawn && CountdownController.Singleton != null)
            {
                CountdownController.Singleton.StartCountdown();
            }
        }
        else
        {
            Debug.LogWarning("Respawn point not set on Killzone!");
        }
    }
}
