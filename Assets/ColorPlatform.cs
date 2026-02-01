using System;
using UnityEngine;

public class ColorPlatform : MonoBehaviour
{
    public GameManager.colors color;

    private void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
    }

    void OnColorChange()
    {
        // If platform color is NONE, always stay active
        if (color == GameManager.colors.NONE)
        {
            gameObject.SetActive(true);
        }
        else if (GameManager.Singleton.currentColor == color)
        {
            gameObject.SetActive(true);
        }
        else 
        {
            gameObject.SetActive(false);
        }
    }
}
