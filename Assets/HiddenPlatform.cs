using UnityEngine;

public class HiddenPlatform : MonoBehaviour
{
    public GameManager.colors color;

    private void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
    }

    void OnColorChange()
    {
        // If platform color is NONE, always stay visible
        if (color == GameManager.colors.NONE)
        {
            gameObject.SetActive(true);
        }
        else if (GameManager.Singleton.currentColor == color)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
