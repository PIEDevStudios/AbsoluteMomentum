using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapIcon : MonoBehaviour
{
    public Image Image;
    public TextMeshProUGUI Text;
    public RectTransform RectTransform;
    public RectTransform IconRectTransform;

    public void SetIcon(Sprite Icon) => Image.sprite = Icon;
    public void setColor(Color color) => Image.color = color;
    public void setText(string text) {
        if(!string.IsNullOrEmpty(text)) {
            Text.enabled = true;
            Text.text = text;
        }
    }
    
    public void SetTextSize(int size) => Text.fontSize = size;
}
