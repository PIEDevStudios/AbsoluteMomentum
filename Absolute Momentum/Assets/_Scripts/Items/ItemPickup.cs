using System;
using DG.Tweening;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerItemManager.ItemType itemType;
    [SerializeField] private float rotationTime = 2f, hoverTime = .8f, hoverDistance = .3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 360f, transform.localEulerAngles.z), rotationTime, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        transform.DOLocalMoveY(transform.localPosition.y + hoverDistance, rotationTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerItemManager itemManager = other.transform.root.GetComponentInChildren<PlayerItemManager>();
            itemManager.SelectItem(itemType);
        }
    }
}
