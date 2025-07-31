using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlideIn : MonoBehaviour {
    public Vector2 startOffset = new Vector2(-300f, 0f);  
    public float duration = 0.5f;
    public float delay = 0f;
    public Ease easing = Ease.OutCubic;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;

    void Awake() {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void PlaySlideIn() {
        rect.anchoredPosition = originalPos + startOffset;
        canvasGroup.alpha = 0f;

        rect.DOAnchorPos(originalPos, duration)
            .SetEase(easing)
            .SetDelay(delay);

        canvasGroup.DOFade(1f, duration)
            .SetEase(Ease.InOutQuad)
            .SetDelay(delay);
    }
}
