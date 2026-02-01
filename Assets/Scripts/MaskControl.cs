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
    
    // Store initial positions for reset
    private Vector3 iconInitialPosition;
    private Vector3[] colorSpritesInitialPositions;
    private Vector3[] colorSpritesVisiblePositions;
    private Coroutine[] spriteCoroutines;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store initial positions
        iconInitialPosition = icon.localPosition;
        colorSpritesInitialPositions = new Vector3[colorSprites.Length];
        colorSpritesVisiblePositions = new Vector3[colorSprites.Length];
        spriteCoroutines = new Coroutine[colorSprites.Length];
        
        for (int i = 0; i < colorSprites.Length; i++)
        {
            colorSpritesInitialPositions[i] = colorSprites[i].localPosition;
            colorSpritesVisiblePositions[i] = new Vector3(
                colorSpritesInitialPositions[i].x - spriteMoveDistance,
                colorSpritesInitialPositions[i].y,
                colorSpritesInitialPositions[i].z
            );
        }
        
        GameManager.Singleton.ColorChangeEvent.AddListener(OnColorChange);
        GameManager.Singleton.OnGameRestart.AddListener(ResetMasks);
    }

    public void ResetMasks()
    {
        // Stop any ongoing animations
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StopAllCoroutines();
        
        // Reset icon position
        icon.localPosition = iconInitialPosition;
        
        // Reset all color sprite positions to hidden
        for (int i = 0; i < colorSprites.Length; i++)
        {
            colorSprites[i].localPosition = colorSpritesInitialPositions[i];
            spriteCoroutines[i] = null;
        }
        
        // Show the current color's sprite
        int currentColor = (int)GameManager.Singleton.currentColor;
        if (currentColor >= 0 && currentColor < colorSprites.Length)
        {
            colorSprites[currentColor].localPosition = colorSpritesVisiblePositions[currentColor];
            currentSpriteIndex = currentColor;
            
            // Move icon to current color position
            icon.localPosition = masks[currentColor].localPosition;
        }
        else
        {
            currentSpriteIndex = -1;
        }
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
        // Stop and animate out the current sprite
        if (currentSpriteIndex >= 0 && currentSpriteIndex < colorSprites.Length)
        {
            if (spriteCoroutines[currentSpriteIndex] != null)
            {
                StopCoroutine(spriteCoroutines[currentSpriteIndex]);
            }
            spriteCoroutines[currentSpriteIndex] = StartCoroutine(AnimateSpriteOut(currentSpriteIndex));
        }
        
        // Stop and animate in the new sprite
        if (newColorIndex >= 0 && newColorIndex < colorSprites.Length)
        {
            if (spriteCoroutines[newColorIndex] != null)
            {
                StopCoroutine(spriteCoroutines[newColorIndex]);
            }
            spriteCoroutines[newColorIndex] = StartCoroutine(AnimateSpriteIn(newColorIndex));
        }
        
        currentSpriteIndex = newColorIndex;
    }

    IEnumerator AnimateSpriteIn(int spriteIndex)
    {
        RectTransform sprite = colorSprites[spriteIndex];
        Vector3 startPosition = sprite.localPosition;
        Vector3 targetPosition = colorSpritesVisiblePositions[spriteIndex];
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
        spriteCoroutines[spriteIndex] = null;
    }

    IEnumerator AnimateSpriteOut(int spriteIndex)
    {
        RectTransform sprite = colorSprites[spriteIndex];
        Vector3 startPosition = sprite.localPosition;
        Vector3 targetPosition = colorSpritesInitialPositions[spriteIndex];
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
        spriteCoroutines[spriteIndex] = null;
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
