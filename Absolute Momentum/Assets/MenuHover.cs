using UnityEngine;

public class MenuHover : MonoBehaviour {
    public float hoverAmount = 5f;
    public float hoverSpeed = 2f;

    private Vector3 startPos;

    void Start() {
        startPos = transform.localPosition;
    }

    void Update() {
        float offset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}
