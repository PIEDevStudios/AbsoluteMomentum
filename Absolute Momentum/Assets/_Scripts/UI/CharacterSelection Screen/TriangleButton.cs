using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TriangleButton : MonoBehaviour
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
    
    public float animationDuration = 0.75f;
    private Vector3 hiddenPosition;
    private Vector2 originalTopRightPos;
    private Vector2 originalBottomLeftPos;
    private Vector3 originalPosition;

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

        confirmButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        originalTopRightPos = topRightTriangle.anchoredPosition;
        originalBottomLeftPos = bottomLeftTriangle.anchoredPosition;
        originalPosition = transform.localPosition;

        confirmButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirm);
        backButton.onClick.AddListener(OnBack);

        SetTriangleRaycast(true);

        nameText.alpha = 1f;
        var color = portraitImage.color;
        color.a = 1f;
        portraitImage.color = color;
    }

    public void OnClicked()
    {
        if (isExpanded || isTweening)
            return;
        
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
            .OnComplete(() => {
                topRightTriangle.DOAnchorPos(originalTopRightPos, animationDuration * 0.2f).SetEase(Ease.OutBack);
            });

        bottomLeftTriangle.DOAnchorPos(overshootBottom, animationDuration * 0.6f).SetEase(Ease.InCubic)
            .OnComplete(() => {
                bottomLeftTriangle.DOAnchorPos(originalBottomLeftPos, animationDuration * 0.2f).SetEase(Ease.OutBack);
                isTweening = false;
            });

        confirmButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        selectManager.ShowAllButtons();
        FadeInNameAndImage();
        SetTriangleRaycast(true);
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

    // public void MoveToHiddenPosition(float duration = 0.5f)
    // {
    //     transform.DOLocalMove(hiddenPosition, duration).SetEase(Ease.InBack);
    // }

}