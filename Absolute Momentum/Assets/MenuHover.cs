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

    void Update() {
        // float offset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        // transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}
