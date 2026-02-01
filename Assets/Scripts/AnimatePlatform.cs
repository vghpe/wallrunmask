using StarterAssets;
using UnityEngine;

public class AnimatePlatform : MonoBehaviour
{
    private Animator animationController;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private float activationDistance = 12.0f;
    public float ModThreshold = 0.1f;
    private bool activateOnce;
    void Start()
    {
        animationController = transform.parent.GetComponent<Animator>();
        if (playerController == null )
        {
            playerController = GameObject.Find("PlayerCapsule").GetComponent<FirstPersonController>();
        }
        GameManager.Singleton.OnGameRestart.AddListener(OnRestart);
    }
    void OnRestart()
    {
        animationController.SetTrigger("Reset");
        activateOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<BoostRamp>().Activated && !activateOnce)
        {
            animationController.ResetTrigger("Reset"); 
            animationController.SetTrigger("Jump");
            activateOnce = true;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.LogWarning("No dash functionality for ramp yet.");
    //}
}
