using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    public GameObject activationTarget;

    public void OnHit()
    {
        gameObject.SetActive(false);

        if (activationTarget != null )
        {
            activationTarget.GetComponent<ActivatedObjectParent>().OnActivation();
        }
    }
}
