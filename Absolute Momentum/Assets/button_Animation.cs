using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Vector3 originalScale;
    private Quaternion originalRotation;
    public float hoverScale = 1.1f;
    public float rotationAngle = 15f;
    public float speed = 5f;

    void Start() {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(AnimateButton(originalScale * hoverScale, Quaternion.Euler(0, 0, rotationAngle)));
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(AnimateButton(originalScale, originalRotation));
    }

    private System.Collections.IEnumerator AnimateButton(Vector3 targetScale, Quaternion targetRot) {
        float t = 0;
        Vector3 startScale = transform.localScale;
        Quaternion startRot = transform.rotation;
        while (t < 1) {
            t += Time.deltaTime * speed;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }
    }
}
