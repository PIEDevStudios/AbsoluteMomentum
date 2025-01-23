using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : State
{
    [SerializeField] private Player player;
    private Vector3 lastPos;
    private PlayerStats stats => player.stats;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.SetTrigger("Idle");
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        rb.linearDamping = stats.GroundDrag;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb.linearDamping = 100;
        }
    }
    
}
