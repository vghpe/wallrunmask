using System;
using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    public GameObject activationTarget;

    // Event that fires when this target is hit
    public event Action<ShootingTarget> onHit;

    public void OnHit()
    {
        // Broadcast the hit event before deactivating
        onHit?.Invoke(this);

        gameObject.SetActive(false);

        if (activationTarget != null)
        {
            activationTarget.GetComponent<ActivatedObjectParent>().OnActivation();
        }
    }
}
