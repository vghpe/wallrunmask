using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public static CountdownController Singleton;

    [Header("Countdown Settings")]
    [Tooltip("The TextMeshPro text to display the countdown")]
    public TextMeshProUGUI countdownText;
    
    [Tooltip("Time in seconds for each number (3, 2, 1)")]
    public float countdownInterval = 1f;
    
    [Tooltip("Optional: Text to show when countdown finishes (e.g., 'GO!')")]
    public string goText = "GO!";
    
    [Tooltip("How long to show the GO text")]
    public float goDisplayTime = 0.5f;
    
    [Header("Start Countdown")]
    [Tooltip("Start countdown when the scene loads")]
    public bool countdownOnStart = true;
    
    [Header("Events")]
    public UnityEvent OnCountdownStart;
    public UnityEvent OnCountdownEnd;
    
    private bool isCountingDown = false;
    private float savedTimeScale = 1f;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        // Hide countdown text initially
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        
        if (countdownOnStart)
        {
            StartCountdown();
        }
    }

    /// <summary>
    /// Starts the countdown. Freezes time and shows 3-2-1-GO.
    /// </summary>
    public void StartCountdown()
    {
        if (!isCountingDown)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        isCountingDown = true;
        
        // Save the current time scale and freeze time
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        OnCountdownStart?.Invoke();
        
        // Show countdown text
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            
            // Count from 3 to 1
            for (int i = 3; i >= 1; i--)
            {
                countdownText.text = i.ToString();
                
                // Wait using unscaledTime (ignores Time.timeScale)
                yield return WaitForRealSeconds(countdownInterval);
            }
            
            // Show GO text
            if (!string.IsNullOrEmpty(goText))
            {
                countdownText.text = goText;
                yield return WaitForRealSeconds(goDisplayTime);
            }
            
            // Hide countdown text
            countdownText.gameObject.SetActive(false);
        }
        else
        {
            // If no text assigned, just wait the full countdown time
            yield return WaitForRealSeconds(countdownInterval * 3 + goDisplayTime);
        }
        
        // Restore time scale
        Time.timeScale = savedTimeScale;
        
        OnCountdownEnd?.Invoke();
        
        isCountingDown = false;
    }

    /// <summary>
    /// Custom wait that uses real time (unscaled) - works even when Time.timeScale = 0
    /// </summary>
    private IEnumerator WaitForRealSeconds(float seconds)
    {
        float endTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < endTime)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Returns true if countdown is currently active
    /// </summary>
    public bool IsCountingDown()
    {
        return isCountingDown;
    }
}
