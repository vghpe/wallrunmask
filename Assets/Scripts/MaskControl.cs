using UnityEngine;

public class MaskControl : MonoBehaviour
{
    [SerializeField] RectTransform[] masks;
    int[] masksIndex = new int[3];
    Vector3 middleVector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
        masksIndex[0] = (int)GameManager.colors.BLUE; //The left one
        masksIndex[1] = (int)GameManager.colors.RED; // The middle one
        masksIndex[2] = (int)GameManager.colors.GREEN; // The right one
        middleVector = masks[1].position;
    }

    void OnColorChange()
    {
        int maskMiddleColorIndex = masksIndex[1];
        int currentColor = (int)GameManager.Singleton.currentColor;
        if (currentColor == maskMiddleColorIndex)
        {
            return;
        }
        int i = 0;
        int[] masksStored = new int[3]{ (int)GameManager.colors.BLUE, (int)GameManager.colors.RED, (int)GameManager.colors.GREEN };
        while (i < masks.Length)
        {
            if (masksStored[i] == currentColor)
            {
                break;
            }
            i++;
        }
        if (i > 2)
        {
            Debug.Log("Out of bounds.");
            return;
        }
        masks[Remap(maskMiddleColorIndex)].position = masks[i].position;
        masks[i].position = middleVector;
        masksIndex[1] = currentColor;
        if (masksIndex[0] == currentColor)
        {
            masksIndex[0] = maskMiddleColorIndex;
        }
        else
        {
            masksIndex[2] = maskMiddleColorIndex;
        }
    }
    int Remap(int index)
    {
        switch (index)
        {
            case (int)GameManager.colors.BLUE:
                return 0;
            case (int)GameManager.colors.RED:
                return 1;
            case (int)GameManager.colors.GREEN:
                return 2;
        }
        return 0;
    }
}
