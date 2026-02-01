using StarterAssets;
using System.Collections;
using UnityEngine;

public class AnimatePlatform : MonoBehaviour
{
    private Animator animationController;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private float activationDistance = 9.0f;
    public float ModThreshold = 0.1f;
    private bool activateOnce;
    void Start()
    {
        animationController = GetComponent<Animator>();
        if (playerController == null )
        {
            playerController = GameObject.Find("PlayerCapsule").GetComponent<FirstPersonController>();
        }
        GameManager.Singleton.OnGameRestart.AddListener(OnRestart);
    }
    void OnRestart()
    {
        activateOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toTarget = transform.position - playerController.transform.position;
        if (toTarget.sqrMagnitude < activationDistance * activationDistance && !activateOnce)
        {
            animationController.SetTrigger("Jump");
            activateOnce = true;
            //Add dash here maybe.
        }
    }
}
