using UnityEngine;
using DG.Tweening;
public class MenuHover : MonoBehaviour {
    public float hoverAmount = 5f;
    public float hoverSpeed = 2f;
    public Ease ease;
    public RectTransform rectTransform;
    private Vector3 startPos;

    void Start() {
        startPos = rectTransform.localPosition;
        rectTransform.DOLocalMoveY(startPos.y + hoverAmount, hoverSpeed).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
    }
}
