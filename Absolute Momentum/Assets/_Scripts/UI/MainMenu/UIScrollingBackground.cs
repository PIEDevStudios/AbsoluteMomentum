using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIScrollingTiledBackground : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private float scrollSpeedX = 0.1f;
    [SerializeField] private float scrollSpeedY = 0.0f;

    void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeedX, scrollSpeedY) * Time.deltaTime, image.uvRect.size);
    }
}