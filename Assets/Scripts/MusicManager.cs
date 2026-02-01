using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Singleton { get; private set; }

    [Header("Music Tracks")]
    [Tooltip("Track that plays when BLUE is active")]
    [SerializeField] private AudioClip blueTrack;
    [Tooltip("Track that plays when RED is active")]
    [SerializeField] private AudioClip redTrack;
    [Tooltip("Track that plays when GREEN is active")]
    [SerializeField] private AudioClip greenTrack;

    [Header("Settings")]
    [SerializeField] private float masterVolume = 1.0f;

    private AudioSource blueSource;
    private AudioSource redSource;
    private AudioSource greenSource;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;

        // Create audio sources
        blueSource = gameObject.AddComponent<AudioSource>();
        redSource = gameObject.AddComponent<AudioSource>();
        greenSource = gameObject.AddComponent<AudioSource>();

        // Configure audio sources
        ConfigureAudioSource(blueSource, blueTrack);
        ConfigureAudioSource(redSource, redTrack);
        ConfigureAudioSource(greenSource, greenTrack);
    }

    private void Start()
    {
        // Subscribe to color change event
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);

        // Start all tracks
        StartAllTracks();

        // Set initial volumes based on current color
        OnColorChange();
    }

    private void ConfigureAudioSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f;
        source.spatialBlend = 0f; // 2D audio (not spatialized)
    }

    private void StartAllTracks()
    {
        blueSource.Play();
        redSource.Play();
        greenSource.Play();
    }

    private void OnColorChange()
    {
        GameManager.colors currentColor = GameManager.Singleton.currentColor;

        // Instant mute/unmute based on active color
        blueSource.volume = (currentColor == GameManager.colors.BLUE) ? masterVolume : 0f;
        redSource.volume = (currentColor == GameManager.colors.RED) ? masterVolume : 0f;
        greenSource.volume = (currentColor == GameManager.colors.GREEN) ? masterVolume : 0f;
    }

    public void RestartAllTracks()
    {
        blueSource.Stop();
        redSource.Stop();
        greenSource.Stop();

        blueSource.time = 0f;
        redSource.time = 0f;
        greenSource.time = 0f;

        StartAllTracks();
        OnColorChange();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        OnColorChange();
    }
}
