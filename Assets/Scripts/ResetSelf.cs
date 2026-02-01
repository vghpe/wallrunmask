using UnityEngine;

public class ResetSelf : MonoBehaviour
{
    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
        GameManager.Singleton.OnGameRestart.AddListener(OnRestart);
    }

    void OnRestart()
    {
        transform.position = startPosition;
    }
}
