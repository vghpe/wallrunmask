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
    private StarterAssetsInputs input;
    private Rigidbody rb;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        
        if (targetCharacter != null)
        {
            characterController = targetCharacter.GetComponent<CharacterController>();
            firstPersonController = targetCharacter.GetComponent<FirstPersonController>();
            input = targetCharacter.GetComponent<StarterAssetsInputs>();
            rb = targetCharacter.GetComponent<Rigidbody>();
        }
        
        wasGrounded = firstPersonController != null ? firstPersonController.Grounded : true;
    }

    private void Update()
    {
        bool isGrounded = firstPersonController != null ? firstPersonController.Grounded : true;
        
        if (isGrounded && !wasGrounded && landingClips != null && landingClips.Length > 0)
        {
            PlayLandingSound();
        }
        
        if (!isGrounded && wasGrounded && jumpClips != null && jumpClips.Length > 0)
        {
            if (input != null && input.jump)
            {
                PlayJumpSound();
            }
        }
        
        wasGrounded = isGrounded;
        
        if (!isGrounded || footstepClips == null || footstepClips.Length == 0)
            return;

        float speed = GetMovementSpeed();

        if (speed > minimumSpeed)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private float GetMovementSpeed()
    {
        if (characterController != null)
        {
            return characterController.velocity.magnitude;
        }
        else if (rb != null)
        {
            return rb.linearVelocity.magnitude;
        }
        
        return 0f;
    }

    private void PlayFootstep()
    {
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
        audioSource.PlayOneShot(clip);
    }

    private void PlayLandingSound()
    {
        AudioClip clip = landingClips[Random.Range(0, landingClips.Length)];
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = Random.Range(volumeRange.y, 1.0f);
        audioSource.PlayOneShot(clip);
    }

    private void PlayJumpSound()
    {
        AudioClip clip = jumpClips[Random.Range(0, jumpClips.Length)];
        audioSource.pitch = 1.0f;
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
        audioSource.PlayOneShot(clip);
    }
}
