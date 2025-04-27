using System;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class SmokeBomb : BaseItem
{
    private Player player;
    [SerializeField] private float throwForce = 70f;
    [SerializeField] private float throwUpwardForce = 10f;
    [SerializeField] private float hoverAmount = 3f, hoverTime = 1f;
    [FormerlySerializedAs("item")] public GameObject projectilePrefab;

    public void Start()
    {
        player = GetComponentInParent<Player>();
        transform.localPosition = new Vector3(transform.localPosition.x, -hoverAmount, transform.localPosition.z);
        transform.DOLocalMoveY(transform.localPosition.y + hoverAmount, hoverTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public override void ActivateItem()
    {
        base.ActivateItem();

        GameObject projectile = Instantiate(projectilePrefab, player.transform.position, player.orientation.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = player.orientation.forward * throwForce + transform.up  * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        Destroy(gameObject);
    }
}
