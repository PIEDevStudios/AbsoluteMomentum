using UnityEngine;
using DG.Tweening;
public class LobbyCircle : MonoBehaviour
{
    [SerializeField] private float spinSpeed, floatHeight, floatTime;

    void Start()
    {
        transform.DOMove(transform.position + new Vector3(0, floatHeight, 0), floatTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + spinSpeed * Time.deltaTime, transform.eulerAngles.z);
    }
}
