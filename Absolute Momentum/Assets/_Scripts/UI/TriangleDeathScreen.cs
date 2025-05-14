using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TriangleDeathScreen : BaseDeathScreen
{
    [SerializeField] private RectTransform blackRect;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float startYPosition = 1000f;
    [SerializeField] private float endYPosition = 0f;
    [SerializeField] private float rotationAngle = 45f; // Angle to rotate the rectangle

    private void Awake()
    {
        Debug.Log("TriangleDeathScreen: Awake called");
        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (blackRect == null)
        {
            Debug.LogError("TriangleDeathScreen: blackRect is not assigned!");
            return;
        }

        var image = blackRect.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("TriangleDeathScreen: No Image component found on blackRect!");
            return;
        }

        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("TriangleDeathScreen: Not placed under a Canvas!");
            return;
        }

        Debug.Log($"TriangleDeathScreen setup valid. Canvas order: {canvas.sortingOrder}, " +
                 $"Image color: {image.color}, Position: {blackRect.anchoredPosition}");
    }

    protected override void OnPlay()
    {
        Debug.Log("TriangleDeathScreen: OnPlay called");
        if (blackRect == null)
        {
            Debug.LogError("TriangleDeathScreen: blackRect is not assigned!");
            return;
        }

        // Set the rotation to create triangle effect
        blackRect.rotation = Quaternion.Euler(0, 0, rotationAngle);
        Debug.Log($"TriangleDeathScreen: Set rotation to {rotationAngle} degrees");
        
        StartCoroutine(AnimateTriangle());
    }

    private IEnumerator AnimateTriangle()
    {
        Debug.Log("TriangleDeathScreen: Starting animation");
        
        // Set initial position
        Vector2 startPos = blackRect.anchoredPosition;
        startPos.y = startYPosition;
        blackRect.anchoredPosition = startPos;
        Debug.Log($"TriangleDeathScreen: Set initial position to {startPos}");
        
        // Make sure the rectangle is visible
        Image image = blackRect.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = 1f;
            image.color = color;
            Debug.Log("TriangleDeathScreen: Set image alpha to 1");
        }
        else
        {
            Debug.LogError("TriangleDeathScreen: No Image component found!");
            yield break;
        }

        Debug.Log("TriangleDeathScreen: Moving shape down");
        
        // Animate down
        float t = 0f;
        Vector2 endPos = startPos;
        endPos.y = endYPosition;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / duration;
            
            // Use easeOutQuad for smooth motion
            float easedProgress = 1f - (1f - normalizedTime) * (1f - normalizedTime);
            
            blackRect.anchoredPosition = Vector2.Lerp(startPos, endPos, easedProgress);
            yield return null;
        }

        Debug.Log($"TriangleDeathScreen: Reached bottom position {blackRect.anchoredPosition}");
        yield return new WaitForSeconds(waitTime);

        Debug.Log("TriangleDeathScreen: Moving shape back up");
        
        // Animate back up
        t = 0f;
        startPos = blackRect.anchoredPosition;
        endPos = startPos;
        endPos.y = startYPosition;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / duration;
            
            // Use easeInQuad for smooth motion
            float easedProgress = normalizedTime * normalizedTime;
            
            blackRect.anchoredPosition = Vector2.Lerp(startPos, endPos, easedProgress);
            yield return null;
        }

        Debug.Log("TriangleDeathScreen: Animation complete, deactivating");
        gameObject.SetActive(false);
    }
} 