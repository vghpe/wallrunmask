using UnityEngine;
using System.Collections;

public class MaskControl : MonoBehaviour
{
    [SerializeField] RectTransform[] masks;
    [SerializeField] RectTransform icon;
    [SerializeField] float moveDuration = 0.3f;
    
    [Header("Color Sprites")]
    [SerializeField] RectTransform[] colorSprites;
    [SerializeField] float spriteMoveDistance = 200f;
    [SerializeField] float spriteMoveDuration = 0.5f;
    
    private Coroutine moveCoroutine;
    private int currentSpriteIndex = -1;

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
        
        // Animate color sprites
        AnimateColorSprites(currentColor);
    }

    void AnimateColorSprites(int newColorIndex)
    {
        // Animate out the current sprite
        if (currentSpriteIndex >= 0 && currentSpriteIndex < colorSprites.Length)
        {
            StartCoroutine(AnimateSpriteOut(colorSprites[currentSpriteIndex]));
        }
        
        // Animate in the new sprite
        if (newColorIndex >= 0 && newColorIndex < colorSprites.Length)
        {
            StartCoroutine(AnimateSpriteIn(colorSprites[newColorIndex]));
        }
        
        currentSpriteIndex = newColorIndex;
    }

    IEnumerator AnimateSpriteIn(RectTransform sprite)
    {
        Vector3 startPosition = sprite.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x - spriteMoveDistance, startPosition.y, startPosition.z);
        float elapsed = 0f;

        while (elapsed < spriteMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spriteMoveDuration;
            
            // Ease out (quadratic)
            float eased = 1f - (1f - t) * (1f - t);
            
            sprite.localPosition = Vector3.Lerp(startPosition, targetPosition, eased);
            yield return null;
        }

        sprite.localPosition = targetPosition;
    }

    IEnumerator AnimateSpriteOut(RectTransform sprite)
    {
        Vector3 startPosition = sprite.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x + spriteMoveDistance, startPosition.y, startPosition.z);
        float elapsed = 0f;

        while (elapsed < spriteMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spriteMoveDuration;
            
            // Ease in (quadratic)
            float eased = t * t;
            
            sprite.localPosition = Vector3.Lerp(startPosition, targetPosition, eased);
            yield return null;
        }

        sprite.localPosition = targetPosition;
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
