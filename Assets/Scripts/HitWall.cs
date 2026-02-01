using UnityEngine;

public class HitWall : MonoBehaviour
{
    [SerializeField] private Killzone killzone;
    private GameObject cells;
    private GameObject wall;
    void Start()
    {
        cells = transform.parent.GetChild(2).gameObject;
        wall = transform.parent.GetChild(1).gameObject;
        if (killzone == null)
        {
            killzone = GameObject.Find("Kill Reset Zone").GetComponent<Killzone>(); 
        }
        GameManager.Singleton.OnGameRestart.AddListener(OnRestart);
    }
    void OnRestart()
    {
        wall.SetActive(true);
        cells.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Singleton.currentColor != GameManager.colors.BLUE)
            {
                if (!cells.activeSelf)
                {
                    killzone.RestartGame(other.gameObject);
                }
            }
        }
    }
}
