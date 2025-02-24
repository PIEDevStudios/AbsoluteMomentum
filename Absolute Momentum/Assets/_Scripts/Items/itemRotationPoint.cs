using UnityEngine;
using DG.Tweening;


public class itemRotationPoint : MonoBehaviour
{
    public float orbitDuration = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), orbitDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
