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
        // In player idle and move states, we set the gravity to 0 so we don't slide down slopes
        player.ChangeGravity(0);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        rb.linearDamping = stats.GroundDrag;
        player.ChangeGravity(stats.NormalGravity);
    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
        if (player.groundSensor.grounded)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    
    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //     {
    //         rb.linearDamping = 100;
    //     }
    // }
    
}
