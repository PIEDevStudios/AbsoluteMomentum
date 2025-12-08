using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class SpeedBoost : BaseItem
{
    [SerializeField] private float hoverAmount = 3f, hoverTime = 1f;
    [SerializeField] private float _speedMultiplier = 0.5f, _speedTime = 5f;

    public void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -hoverAmount, transform.localPosition.z);
        transform.DOLocalMoveY(transform.localPosition.y + hoverAmount, hoverTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public override void ActivateItem()
    {
        ActivateItemServerRpc();
        Player.StartCoroutine(ChangeSpeed());
        Destroy(gameObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ActivateItemServerRpc()
    {
        if (!IsServer) return;
    }

    private IEnumerator ChangeSpeed()
    {
        Player.SpeedMultiplier += _speedMultiplier;
        yield return new WaitForSeconds(_speedTime);
        Player.SpeedMultiplier -= _speedMultiplier;
    }
    
}