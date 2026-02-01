using StarterAssets;
using UnityEngine;

public class AnimateCharacter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private FirstPersonController firstPersonController;
    Animator animationController;
    void Start()
    {
        animationController = GetComponent<Animator>();
        GameManager.Singleton.ColorChangeEvent.AddListener(ChangeMask);
        firstPersonController = transform.parent.GetComponent<FirstPersonController>();
    }

    private bool wasWallRunning = false;
    private int lastWallSide = 0;

    private void Update()
    {
        bool isWallRunning = firstPersonController.WallRun;
        int wallSide = firstPersonController.WallSide;

        // Om wallrun-status ändrats
        if (isWallRunning != wasWallRunning || wallSide != lastWallSide)
        {
            // Nollställ båda först (säkert)
            animationController.SetBool("Left_Running", false);
            animationController.SetBool("Right_Running", false);

            if (isWallRunning)
            {
                if (wallSide == 1)
                    animationController.SetBool("Right_Running", true);
                else
                    animationController.SetBool("Left_Running", true);
            }

            // Spara state
            wasWallRunning = isWallRunning;
            lastWallSide = wallSide;
        }
    }

    public void Shoot()
    {
        animationController.SetTrigger("shoot");
    }

    void ChangeMask()
    {
        animationController.SetTrigger("ChangeMask");
    }
}
