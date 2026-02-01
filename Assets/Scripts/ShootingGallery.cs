using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootingGallery : MonoBehaviour
{
    [Header("Targets")]
    public List<ShootingTarget> targets = new List<ShootingTarget>();

    [Header("Events")]
    public UnityEvent onAllTargetsHit;

    [Header("Optional Activation")]
    public GameObject activationTarget;
    
    [Header("Hit Effect")]
    [Tooltip("Particle system prefab to spawn when a target is hit")]
    public GameObject hitParticlePrefab;
    [Tooltip("Auto-destroy particle system after this many seconds (0 = don't auto-destroy)")]
    public float particleLifetime = 2f;

    private HashSet<ShootingTarget> hitTargets = new HashSet<ShootingTarget>();

    void Start()
    {
        // Subscribe to game restart event
        if (GameManager.Singleton != null)
        {
            GameManager.Singleton.OnGameRestart.AddListener(ResetGallery);
            //Debug.Log("ShootingGallery: Subscribed to OnGameRestart");
        }
        else
        {
            //Debug.LogWarning("ShootingGallery: GameManager.Singleton is null!");
        }
    }

    void OnEnable()
    {
        // Subscribe to each target's hit event
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.onHit += OnTargetHit;
            }
        }
    }

    void OnDisable()
    {
        // Unsubscribe from each target's hit event
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.onHit -= OnTargetHit;
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from game restart event
        if (GameManager.Singleton != null)
        {
            GameManager.Singleton.OnGameRestart.RemoveListener(ResetGallery);
        }
    }

    void OnTargetHit(ShootingTarget target)
    {
        // Spawn hit particle effect at target location
        if (hitParticlePrefab != null && target != null)
        {
            GameObject particleInstance = Instantiate(hitParticlePrefab, target.transform.position, Quaternion.Euler(90, 0, 0));
            
            if (particleLifetime > 0)
            {
                Destroy(particleInstance, particleLifetime);
            }
        }
        
        hitTargets.Add(target);

        if (hitTargets.Count >= targets.Count)
        {
            OnAllTargetsHit();
        }
    }

    void OnAllTargetsHit()
    {
        //Debug.Log("ShootingGallery: All targets hit!");

        // Invoke the Unity Event (for inspector-assigned callbacks)
        onAllTargetsHit?.Invoke();

        // Activate target if assigned (same pattern as ShootingTarget)
        if (activationTarget != null)
        {
            var activatedObject = activationTarget.GetComponent<ActivatedObjectParent>();
            if (activatedObject != null)
            {
                activatedObject.OnActivation();
            }
        }
    }

    public void ResetGallery()
    {
        //Debug.Log("ShootingGallery: ResetGallery called!");
        
        hitTargets.Clear();

        // Reactivate all targets
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.gameObject.SetActive(true);
                //Debug.Log("ShootingGallery: Reactivated target " + target.name);
            }
        }
    }

    public int GetHitCount()
    {
        return hitTargets.Count;
    }

    public int GetTotalTargets()
    {
        return targets.Count;
    }
}
