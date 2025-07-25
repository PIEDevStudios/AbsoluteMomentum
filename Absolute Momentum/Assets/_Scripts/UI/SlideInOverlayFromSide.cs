using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlideInOverlayFromSide : BaseDeathScreen
{
    [SerializeField] private RectTransform overlayImageRect;
    [SerializeField] private float waitAfterSlide = 1f;
    [SerializeField] private float startXPosition = -2000f;  // Off-screen left by default
    [SerializeField] private float endXPosition = 0f;

    private Image overlayImage;

    private void Awake()
    {
        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (overlayImageRect == null)
            return;

        overlayImage = overlayImageRect.GetComponent<Image>();
        if (overlayImage == null)
            return;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;
    }

    protected override void OnPlay()
    {
        if (overlayImageRect == null || overlayImage == null)
            return;

        StartCoroutine(PlaySlideInAnimation());
    }

    private IEnumerator PlaySlideInAnimation()
    {
        Vector2 startPos = overlayImageRect.anchoredPosition;
        startPos.x = startXPosition;
        overlayImageRect.anchoredPosition = startPos;

        Color color = overlayImage.color;
        color.a = 1f;
        overlayImage.color = color;

        float t = 0f;
        Vector2 endPos = startPos;
        endPos.x = endXPosition;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);
            float eased = 1f - (1f - progress) * (1f - progress); // easeOutQuad
            overlayImageRect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            yield return null;
        }

        if (waitAfterSlide > 0f)
            yield return new WaitForSeconds(waitAfterSlide);
    }
}
