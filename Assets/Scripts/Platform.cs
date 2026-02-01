using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum PlatformMode
    {
        AlwaysVisible,      // NONE - always active
        ShowWhenColorMatch, // Like ColorPlatform - visible when color matches
        HideWhenColorMatch  // Like HiddenPlatform - hidden when color matches
    }
    
    public PlatformMode mode = PlatformMode.ShowWhenColorMatch;
    public GameManager.colors color;
    
    [Header("Visual Children")]
    [Tooltip("Child object to show when platform is ACTIVE")]
    public GameObject activeVisual;
    
    [Tooltip("Child object to show when platform is INACTIVE")]
    public GameObject inactiveVisual;
    
    private Collider[] allColliders;

    private void Awake()
    {
        // Cache all colliders on this object AND children
        allColliders = GetComponentsInChildren<Collider>(true);
    }

    private void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
        OnColorChange(); // Apply initial state
    }

    private void OnEnable()
    {
        // Re-apply state when enabled in case we missed an event
        OnColorChange();
    }

    void OnColorChange()
    {
        if (GameManager.Singleton == null)
            return;
            
        bool shouldBeActive = ShouldBeActive();
        
        // Toggle colliders
        foreach (Collider col in allColliders)
        {
            if (col != null)
                col.enabled = shouldBeActive;
        }
        
        // Toggle visuals
        if (activeVisual != null)
            activeVisual.SetActive(shouldBeActive);
        
        if (inactiveVisual != null)
            inactiveVisual.SetActive(!shouldBeActive);
    }
    
    private bool ShouldBeActive()
    {
        switch (mode)
        {
            case PlatformMode.AlwaysVisible:
                return true;
                
            case PlatformMode.ShowWhenColorMatch:
                if (color == GameManager.colors.NONE)
                    return true;
                return GameManager.Singleton.currentColor == color;
                
            case PlatformMode.HideWhenColorMatch:
                if (color == GameManager.colors.NONE)
                    return true;
                return GameManager.Singleton.currentColor != color;
                
            default:
                return true;
        }
    }
}
