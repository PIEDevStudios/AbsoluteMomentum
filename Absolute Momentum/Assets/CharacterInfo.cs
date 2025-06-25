using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterData {
    public string characterName;
    public Sprite characterImage;
    public string description;
}

public class CharacterInfo : MonoBehaviour {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image portraitImage;

    public SlideInDotTween titleSlide;
    public SlideInDotTween descriptionSlide;
    public SlideInDotTween portraitSlide;

    public void UpdateInfo(CharacterData data)
    {
        nameText.text = data.characterName;
        descriptionText.text = data.description;
        portraitImage.sprite = data.characterImage;

        titleSlide.PlaySlideIn();
        descriptionSlide.PlaySlideIn();
        portraitSlide.PlaySlideIn();
    }
}
