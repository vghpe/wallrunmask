using UnityEngine;

public class AxisFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("The target to follow")]
    public Transform target;
    
    [Header("Axis Selection")]
    [Tooltip("Follow on X axis")]
    public bool followX = false;
    
    [Tooltip("Follow on Y axis")]
    public bool followY = false;
    
    [Tooltip("Follow on Z axis")]
    public bool followZ = true;
    
    [Header("Optional Offset")]
    [Tooltip("Offset from target position")]
    public Vector3 offset = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 newPosition = transform.position;
        Vector3 targetPosition = target.position + offset;

        if (followX)
            newPosition.x = targetPosition.x;
        
        if (followY)
            newPosition.y = targetPosition.y;
        
        if (followZ)
            newPosition.z = targetPosition.z;

        transform.position = newPosition;
    }
}
