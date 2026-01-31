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
        if (GameManager.Singleton.currentColor == color)
        {
            gameObject.SetActive(true);
        }
        else 
        {
            gameObject.SetActive(false);
        }
    }
}
