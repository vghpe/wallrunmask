using UnityEngine;

public class HandleMaskVisibility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject[] masks;
    [SerializeField] private Animator myCharacter;
    private int previous;
    private bool changeToggle;
    private void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(ChangeColor);
        previous = (int)GameManager.Singleton.currentColor;
    }
    void ChangeColor()
    {
        transform.GetChild(previous).gameObject.SetActive(false);
        previous = (int)GameManager.Singleton.currentColor;
        transform.GetChild(previous).gameObject.SetActive(true);
    }
}
