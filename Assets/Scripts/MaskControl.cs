using UnityEngine;

public class MaskControl : MonoBehaviour
{
    [SerializeField] RectTransform[] masks;
    [SerializeField] RectTransform icon;
    //readonly int[] masksStored = new int[3] { (int)GameManager.colors.BLUE, (int)GameManager.colors.RED, (int)GameManager.colors.GREEN };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
    }

    void OnColorChange()
    {
        int currentColor = (int)GameManager.Singleton.currentColor;
        icon.anchoredPosition = masks[currentColor].anchoredPosition;
    }
}
