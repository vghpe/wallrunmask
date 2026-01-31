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
        BLUE
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

        ColorChangeEvent.Invoke();
    }
}
