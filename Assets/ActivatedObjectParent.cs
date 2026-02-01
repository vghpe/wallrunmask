using UnityEngine;

public class ActivatedObjectParent : MonoBehaviour
{
    public bool Activated;
    public void OnActivation()
    {
        Activated = true;
        Debug.Log("I have been activated");
    }
}
