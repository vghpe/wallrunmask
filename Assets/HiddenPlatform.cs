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
        if (GameManager.Singleton.currentColor == color)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
