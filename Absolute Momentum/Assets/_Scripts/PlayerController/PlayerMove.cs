using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;

public class PlayerMove : State
{
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation, colliderPivot; 
    public State runState;
    public State crouchState;
    private Vector3 enterHitboxScale;
    private float maxSpeed;
    private float acceleration;
    
    private StudioEventEmitter footstepEmitter;
    private PlayerStats stats => player.stats;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        stateMachine.SetState(player.playerInput.slideHeld ? crouchState : runState, true);
        enterHitboxScale = colliderPivot.localScale;
        rb.linearDamping = stats.GroundDrag;
        player.ChangeGravity(0);
        player.playerSpeedManager.currentCurve = stats.groundDragCurve;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        colliderPivot.localScale = enterHitboxScale;
        player.ChangeGravity(stats.NormalGravity);
    }

    public override void DoUpdateState()
    {
        stateMachine.SetState(player.playerInput.slideHeld ? crouchState : runState);
        colliderPivot.localScale = player.playerInput.slideHeld ? player.stats.slidePlayerScale : enterHitboxScale;
        base.DoUpdateState();
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        
        StickToSlope();
        
        RaycastHit hit = player.slopeSensor.hit;
        Vector3 forwardOriented = Vector3.Cross(orientation.right, hit.normal).normalized;
        Vector3 rightOriented = Vector3.Cross(hit.normal, forwardOriented).normalized;
        
        float acceleration = player.playerInput.slideHeld ? stats.CrouchAcceleration : stats.SprintAcceleration;
        
        // Adds a force to the player in the direction they are pressing relative to the camera
        Debug.DrawRay(transform.position, (forwardOriented * player.playerInput.moveVector.y + rightOriented * player.playerInput.moveVector.x).normalized * (stats.SprintAcceleration * 100f), Color.green);
        rb.AddForce((forwardOriented * player.playerInput.moveVector.y + rightOriented * player.playerInput.moveVector.x).normalized * (acceleration * 100f));
        
        NoInputDeceleration();
        
    }

    /// <summary>
    /// If the player's ground check is not on the ground but the slope cast is on the ground, apply a downward force to stick the player to the slope
    /// </summary>
    private void StickToSlope()
    {
        if (!player.groundSensor.grounded && player.slopeSensor.isOnSlope)
        {
            Debug.Log("Stick to slope");
            player.ChangeGravity(0);
            rb.AddForce(Vector3.down * stats.StickToSlopeForce);
        }
        else if (player.groundSensor.grounded && !player.slopeSensor.isOnSlope)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }
    
    private void NoInputDeceleration()
    {
        
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        // If player is not pressing any move button, decelerate them
        if (player.playerInput.moveVector.magnitude == 0f)
        {
            Debug.DrawRay(player.transform.position, -flatVel.normalized, Color.blue);
            rb.AddForce(-flatVel.normalized * stats.GroundNoInputDeceleration);
        }
        // If our velocity is close to 0 and still not pressing an input, set velo to 0
        if (player.playerInput.moveVector.magnitude == 0f && flatVel.magnitude < 3f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            isComplete = true;
        }
    }

}
