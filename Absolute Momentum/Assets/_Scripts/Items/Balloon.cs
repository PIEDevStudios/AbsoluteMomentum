using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Baloon : BaseItem
{
    [SerializeField] private float _itemTime, _hoverStrength;

    public override void ActivateItem()
    {
        ActivateItemServerRpc();
        Player.StartCoroutine(AddBalloonForce());
        Destroy(gameObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ActivateItemServerRpc()
    {
        if (!IsServer) return;
    }
    
    private IEnumerator AddBalloonForce()
    {
        float timer = 0f;
        Player.rb.linearVelocity = new Vector3(Player.rb.linearVelocity.x, 0f, Player.rb.linearVelocity.z);
        while (timer < _itemTime)
        {
            Player.rb.AddForce(Vector3.up * _hoverStrength, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
    }
    
}