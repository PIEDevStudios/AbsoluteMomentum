using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Balloon : BaseItem
{
    [SerializeField] private float _itemTime, _hoverStrength;

    public override void ActivateItem()
    {
        Player.StartCoroutine(AddBalloonForce());
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