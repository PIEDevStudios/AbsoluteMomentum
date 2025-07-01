using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class CharacterData {
    public string characterName;
    public Sprite iconImage;
    public Sprite characterImage;
    public string description;
}

public class CharacterInfo : MonoBehaviour {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image portraitImage;
    public RectTransform panelTransform;

    public float scaleDuration = 0.3f;

    public SlideInDotTween titleSlide;
    public SlideInDotTween descriptionSlide;
    public SlideInDotTween portraitSlide;

    void Awake() {
        panelTransform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void UpdateInfo(CharacterData data) {
        nameText.text = data.characterName;
        descriptionText.text = data.description;
        portraitImage.sprite = data.characterImage;

        PlayShowAnimation();
        titleSlide.PlaySlideIn();
        descriptionSlide.PlaySlideIn();
        portraitSlide.PlaySlideIn();
    }

    public void PlayShowAnimation()
    {
        gameObject.SetActive(true);
        panelTransform.localScale = Vector3.zero;

        panelTransform
            .DOScale(1f, scaleDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(0.35f); 
    }

    public void HidePanel() {
        panelTransform.DOScale(0f, scaleDuration).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
