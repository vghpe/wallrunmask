using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public float size = 20f;
    public float thickness = 2f;
    public float gap = 5f;
    public Color color = Color.white;

    [Header("References (auto-created if empty)")]
    public Image topLine;
    public Image bottomLine;
    public Image leftLine;
    public Image rightLine;
    public Image centerDot;

    [Header("Optional")]
    public bool showCenterDot = true;

    void Start()
    {
        if (topLine == null)
        {
            CreateCrosshair();
        }
        UpdateCrosshair();
    }

    void CreateCrosshair()
    {
        // Create the crosshair lines as child UI Images
        topLine = CreateLine("TopLine");
        bottomLine = CreateLine("BottomLine");
        leftLine = CreateLine("LeftLine");
        rightLine = CreateLine("RightLine");
        centerDot = CreateLine("CenterDot");
    }

    Image CreateLine(string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform, false);
        Image img = lineObj.AddComponent<Image>();
        img.color = color;
        return img;
    }

    public void UpdateCrosshair()
    {
        // Top line
        topLine.rectTransform.sizeDelta = new Vector2(thickness, size);
        topLine.rectTransform.anchoredPosition = new Vector2(0, gap + size / 2);
        topLine.color = color;

        // Bottom line
        bottomLine.rectTransform.sizeDelta = new Vector2(thickness, size);
        bottomLine.rectTransform.anchoredPosition = new Vector2(0, -gap - size / 2);
        bottomLine.color = color;

        // Left line
        leftLine.rectTransform.sizeDelta = new Vector2(size, thickness);
        leftLine.rectTransform.anchoredPosition = new Vector2(-gap - size / 2, 0);
        leftLine.color = color;

        // Right line
        rightLine.rectTransform.sizeDelta = new Vector2(size, thickness);
        rightLine.rectTransform.anchoredPosition = new Vector2(gap + size / 2, 0);
        rightLine.color = color;

        // Center dot
        centerDot.rectTransform.sizeDelta = new Vector2(thickness, thickness);
        centerDot.rectTransform.anchoredPosition = Vector2.zero;
        centerDot.color = color;
        centerDot.gameObject.SetActive(showCenterDot);
    }

    // Call this if you want to change crosshair at runtime
    public void SetColor(Color newColor)
    {
        color = newColor;
        UpdateCrosshair();
    }

    public void SetSize(float newSize)
    {
        size = newSize;
        UpdateCrosshair();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (topLine != null)
        {
            UpdateCrosshair();
        }
    }
#endif
}
