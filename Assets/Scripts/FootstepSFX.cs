using UnityEngine;
using StarterAssets;

public class FootstepSFX : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Array of footstep sound clips for variation")]
    public AudioClip[] footstepClips;
    
    [Tooltip("Array of landing sound clips for variation")]
    public AudioClip[] landingClips;
    
    [Tooltip("Array of jump sound clips for variation")]
    public AudioClip[] jumpClips;
    
    [Header("Playback Settings")]
    [Tooltip("Time between footsteps when walking")]
    public float stepInterval = 0.5f;
    
    [Tooltip("Minimum movement speed to play footsteps")]
    public float minimumSpeed = 0.1f;
    
    [Header("Variation Settings")]
    [Tooltip("Random pitch variation range")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    
    [Tooltip("Random volume variation range")]
    public Vector2 volumeRange = new Vector2(0.8f, 1.0f);
    
    [Header("References")]
    [Tooltip("GameObject with the CharacterController (usually the parent)")]
    public GameObject targetCharacter;
    
    private AudioSource audioSource;
    private float stepTimer;
    private bool wasGrounded;
    private CharacterController characterController;
    private FirstPersonController firstPersonController;
    private Rigidbody rb;

    private void Awake()
    {
        // Get or add AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource for footsteps
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound for first person
        
        // Get movement components from target character
        if (targetCharacter != null)
        {
            characterController = targetCharacter.GetComponent<CharacterController>();
            firstPersonController = targetCharacter.GetComponent<FirstPersonController>();
            rb = targetCharacter.GetComponent<Rigidbody>();
        }
        
        // Initialize grounded state
        wasGrounded = firstPersonController != null ? firstPersonController.Grounded : true;
    }

    private void Update()
    {
        // Check if grounded using FirstPersonController if available
        bool isGrounded = firstPersonController != null ? firstPersonController.Grounded : true;
        
        // Detect landing (transition from not grounded to grounded)
        if (isGrounded && !wasGrounded && landingClips != null && landingClips.Length > 0)
        {
            PlayLandingSound();
        }
        
        // Detect jump (transition from grounded to not grounded)
        if (!isGrounded && wasGrounded && jumpClips != null && jumpClips.Length > 0)
        {
            PlayJumpSound();
        }
        
        // Update previous grounded state
        wasGrounded = isGrounded;
        
        if (!isGrounded || footstepClips == null || footstepClips.Length == 0)
            return;

        // Get current movement speed
        float speed = GetMovementSpeed();

        // Check if moving fast enough
        if (speed > minimumSpeed)
        {
            stepTimer += Time.deltaTime;

            // Play footstep when timer exceeds interval
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset timer when not moving
            stepTimer = 0f;
        }
    }

    private float GetMovementSpeed()
    {
        // Try CharacterController first
        if (characterController != null)
        {
            return characterController.velocity.magnitude;
        }
        // Fall back to Rigidbody
        else if (rb != null)
        {
            return rb.linearVelocity.magnitude;
        }
        
        return 0f;
    }

    private void PlayFootstep()
    {
        // Pick a random clip
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        
        // Apply random pitch variation
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        
        // Apply random volume variation
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
        
        // Play the clip
        audioSource.PlayOneShot(clip);
    }

    private void PlayLandingSound()
    {
        // Pick a random landing clip
        AudioClip clip = landingClips[Random.Range(0, landingClips.Length)];
        
        // Apply random pitch variation
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        
        // Landing sounds are usually slightly louder than footsteps
        audioSource.volume = Random.Range(volumeRange.y, 1.0f);
        
        // Play the clip
        audioSource.PlayOneShot(clip);
    }

    private void PlayJumpSound()
    {
        // Pick a random jump clip
        AudioClip clip = jumpClips[Random.Range(0, jumpClips.Length)];
        
        // Apply random pitch variation
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        
        // Apply random volume variation
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
        
        // Play the clip
        audioSource.PlayOneShot(clip);
    }
}
