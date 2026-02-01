using UnityEngine;

public class ExplodeWall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject cells;
    void Start()
    {
        cells = transform.parent.GetChild(2).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Singleton.currentColor == GameManager.colors.BLUE)
            {
                cells.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
