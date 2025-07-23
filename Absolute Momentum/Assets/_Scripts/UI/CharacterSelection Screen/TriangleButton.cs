using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TriangleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform topRightTriangle;
    public RectTransform bottomLeftTriangle;
    public TextMeshProUGUI nameText;
    public Image portraitImage;

    public Button confirmButton;
    public Button backButton;

    [Header("Top Right Triangle Offset")]
    public float topTriangleOffsetX = 150f;
    public float topTriangleOffsetY = 150f;

    [Header("Bottom Left Triangle Offset")]
    public float bottomTriangleOffsetX = 150f;
    public float bottomTriangleOffsetY = 150f;

    [Header("Hover Settings")]
    public Image hoverIcon;
    public Vector2 hoverOffset = new Vector2(0, -0.5f);
    public float hoverExpandDistance = 50f;
    public Vector2 smallSquareSize = new Vector2(65f, 65f);
    public Vector2 expandedIconSize = new Vector2(150f, 150f);
    public Color squareColor = Color.white; 

    public float animationDuration = 0.75f;
    private Vector3 hiddenPosition;
    private Vector2 originalTopRightPos;
    private Vector2 originalBottomLeftPos;
    private Vector3 originalPosition;

    private Sprite characterHoverIcon;

    private CharacterSelectManager selectManager;
    private int characterIndex;
    private bool isExpanded = false;
    private bool isTweening = false;

    public void Initialize(CharacterData data, int index, CharacterSelectManager manager)
    {
        nameText.text = data.characterName;
        portraitImage.sprite = data.iconImage;

        characterIndex = index;
        selectManager = manager;

        characterHoverIcon = data.hoverIconImage;

        confirmButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        originalTopRightPos = topRightTriangle.anchoredPosition;
        originalBottomLeftPos = bottomLeftTriangle.anchoredPosition;
        originalPosition = transform.localPosition;

        confirmButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirm);
        backButton.onClick.AddListener(OnBack);

        if (hoverIcon != null)
        {
            // Set up as small colored square initially
            hoverIcon.sprite = null; // Remove sprite to show solid color
            hoverIcon.color = squareColor;
            hoverIcon.rectTransform.sizeDelta = smallSquareSize;
            hoverIcon.rectTransform.anchoredPosition = (originalTopRightPos + originalBottomLeftPos) / 2 + hoverOffset;
            hoverIcon.gameObject.SetActive(true); // Keep it visible as a small square
            hoverIcon.rectTransform.localScale = Vector3.one;
        }

        SetTriangleRaycast(true);

        nameText.alpha = 1f;
        var color = portraitImage.color;
        color.a = 1f;
        portraitImage.color = color;
    }

    // Method to change square color at runtime
    public void SetSquareColor(Color newColor)
    {
        squareColor = newColor;
        if (hoverIcon != null && hoverIcon.sprite == null)
        {
            hoverIcon.color = squareColor;
        }
    }

    public void OnClicked()
    {
        if (isExpanded || isTweening)
            return;

        ResetToSquare();
        isExpanded = true;
        isTweening = true;
        selectManager.OnCharacterSelected(characterIndex);
        selectManager.HideOtherButtons(this);

        Vector2 topTriangleSplitOffset = new Vector2(topTriangleOffsetX, topTriangleOffsetY);
        Vector2 bottomTriangleSplitOffset = new Vector2(bottomTriangleOffsetX, bottomTriangleOffsetY);

        float delay = 0.2f;

        topRightTriangle
            .DOAnchorPos(originalTopRightPos + topTriangleSplitOffset, animationDuration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay);

        bottomLeftTriangle
            .DOAnchorPos(originalBottomLeftPos - bottomTriangleSplitOffset, animationDuration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay)
            .OnComplete( () => { isTweening = false; });

        confirmButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);

        FadeOutNameAndImage();
        SetTriangleRaycast(false);

        if (hoverIcon != null)
        {
            hoverIcon.gameObject.SetActive(false);
        }
    }

    private void OnConfirm()
    {
        Debug.Log("Character Confirmed: " + nameText.text);
    }

    private void OnBack()
    {
        if (!isExpanded || isTweening) return;
        
        isTweening = true;
        isExpanded = false;

        Vector2 overshootTop = originalTopRightPos - new Vector2(15f, 15f);
        Vector2 overshootBottom = originalBottomLeftPos + new Vector2(15f, 15f);

        topRightTriangle.DOAnchorPos(overshootTop, animationDuration * 0.6f).SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                topRightTriangle.DOAnchorPos(originalTopRightPos, animationDuration * 0.2f).SetEase(Ease.OutBack);
            });

        bottomLeftTriangle.DOAnchorPos(overshootBottom, animationDuration * 0.6f).SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                bottomLeftTriangle.DOAnchorPos(originalBottomLeftPos, animationDuration * 0.2f).SetEase(Ease.OutBack);
                isTweening = false;
            });

        confirmButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        selectManager.ShowAllButtons();
        FadeInNameAndImage();
        SetTriangleRaycast(true);

        if (hoverIcon != null)
        {
            ResetToSquare();
            hoverIcon.gameObject.SetActive(true);
        }
    }

    private void SetTriangleRaycast(bool enabled)
    {
        var topImage = topRightTriangle.GetComponent<Image>();
        var bottomImage = bottomLeftTriangle.GetComponent<Image>();

        if (topImage) topImage.raycastTarget = enabled;
        if (bottomImage) bottomImage.raycastTarget = enabled;
    }

    private void FadeOutNameAndImage()
    {
        nameText.DOFade(0f, 0.3f).SetEase(Ease.OutQuad);
        var color = portraitImage.color;
        color.a = 1f;
        portraitImage.color = color;
        portraitImage.DOFade(0f, 0.3f).SetEase(Ease.OutQuad);
    }

    private void FadeInNameAndImage()
    {
        nameText.DOFade(1f, 0.3f).SetEase(Ease.InQuad);
        var color = portraitImage.color;
        color.a = 0f;
        portraitImage.color = color;
        portraitImage.DOFade(1f, 0.3f).SetEase(Ease.InQuad);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isExpanded) return;

        ExpandToHoverIcon();
        AnimateTrianglesHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isExpanded) return;

        ResetToSquare();
        AnimateTrianglesHover(false);
    }
    
    private void ExpandToHoverIcon()
    {
        if (hoverIcon != null && characterHoverIcon != null)
        {
            hoverIcon.rectTransform.DOKill();
            
            hoverIcon.sprite = characterHoverIcon;
            hoverIcon.color = Color.white; 
            
            hoverIcon.rectTransform
                .DOSizeDelta(expandedIconSize, 0.3f)
                .SetEase(Ease.OutBack);
        }
    }

    private void ResetToSquare()
    {
        if (hoverIcon != null)
        {
            hoverIcon.rectTransform.DOKill();
            
            hoverIcon.sprite = null;
            hoverIcon.color = squareColor;
            
            hoverIcon.rectTransform
                .DOSizeDelta(smallSquareSize, 0.2f)
                .SetEase(Ease.InBack);
        }
    }
    
    private void AnimateTrianglesHover(bool expand)
    {
        Vector2 hoverExpansion = expand ? new Vector2(hoverExpandDistance, hoverExpandDistance) : Vector2.zero;

        topRightTriangle
            .DOAnchorPos(originalTopRightPos + hoverExpansion, 0.3f)
            .SetEase(Ease.OutSine);

        bottomLeftTriangle
            .DOAnchorPos(originalBottomLeftPos - hoverExpansion, 0.3f)
            .SetEase(Ease.OutSine);
    }
}