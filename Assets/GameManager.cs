using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent ColorChangeEvent;
    public static GameManager Singleton;

    public enum colors
    {
        RED,
        GREEN,
        BLUE,
        NONE
    }

    public colors currentColor;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }

        if (ColorChangeEvent == null)
        {
            ColorChangeEvent = new UnityEvent();
        }
    }
    private void Start()
    {
        currentColor = colors.RED;
        
        // Delay the initial invoke to ensure all platforms have subscribed
        Invoke(nameof(InvokeColorChange), 0.1f);
    }
    
    private void InvokeColorChange()
    {
        ColorChangeEvent.Invoke();
    }
}
