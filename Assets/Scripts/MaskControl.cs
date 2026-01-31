using UnityEngine;
using System.Collections;

public class MaskControl : MonoBehaviour
{
    [SerializeField] RectTransform[] masks;
    [SerializeField] RectTransform icon;
    [SerializeField] float moveDuration = 0.3f;
    
    private Coroutine moveCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
    }

    void OnColorChange()
    {
        int currentColor = (int)GameManager.Singleton.currentColor;
        Vector3 targetPosition = masks[currentColor].localPosition;
        
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveIcon(targetPosition));
    }

    IEnumerator MoveIcon(Vector3 targetPosition)
    {
        Vector3 startPosition = icon.localPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            
            // Ease in (quadratic)
            float eased = t * t;
            
            icon.localPosition = Vector3.Lerp(startPosition, targetPosition, eased);
            yield return null;
        }

        icon.localPosition = targetPosition;
    }
}
