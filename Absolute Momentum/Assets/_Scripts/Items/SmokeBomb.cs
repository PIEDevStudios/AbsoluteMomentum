using System;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class SmokeBomb : BaseItem
{
    [SerializeField] private float throwForce = 70f;
    [SerializeField] private float throwUpwardForce = 10f;
    [SerializeField] private float hoverAmount = 3f, hoverTime = 1f;
    public GameObject projectilePrefab;

    public void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -hoverAmount, transform.localPosition.z);
        transform.DOLocalMoveY(transform.localPosition.y + hoverAmount, hoverTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public override void ActivateItem()
    {
        base.ActivateItem();
        NetworkObject projectile = Instantiate(projectilePrefab, player.transform.position, player.orientation.rotation).GetComponent<NetworkObject>();
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectile.Spawn(true);
        Vector3 forceToAdd = player.orientation.forward * throwForce + transform.up  * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        Destroy(gameObject);
    }
}
