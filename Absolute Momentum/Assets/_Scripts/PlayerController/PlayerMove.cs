using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using FMODUnity;

public class PlayerMove : State
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    private float maxSpeed;
    private float acceleration;

    private StudioEventEmitter footstepEmitter;
    private PlayerStats stats => player.stats;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        rb.linearDamping = stats.GroundDrag;
        player.ChangeGravity(0);
        rb.AddForce(-player.groundSensor.hit.normal * 2f, ForceMode.Impulse);
        // footstepEmitter = AudioManager.Instance.InitializeEventEmitter(FMODEvents.Sounds.PlayerFootsteps_Grass, player.playerObj.gameObject);
        // footstepEmitter.Play();
        // player.ChangeGravity(stats.GroundGravity);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.ChangeGravity(stats.NormalGravity);
        
        // player.ChangeGravity(stats.NormalGravity);
    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
        CheckForSprint();
        LimitVelocity();
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        RaycastHit hit = player.slopeSensor.hit;
        Vector3 forwardOriented = Vector3.Cross(orientation.right, hit.normal).normalized;
        Vector3 rightOriented = Vector3.Cross(hit.normal, forwardOriented).normalized;
        // Adds a force to the player in the direction they are pressing relative to the camera
        //Debug.Log("MOVE FIXED UPDATE");
        rb.AddForce((forwardOriented * playerInput.moveVector.y + rightOriented * playerInput.moveVector.x).normalized * (acceleration * 100f));
        LimitVelocity();
        StickToSlope();
    }

    /// <summary>
    /// Check if player is sprinting
    /// </summary>
    private void CheckForSprint()
    {
        if (!playerInput.sprintHeld)
        {
            maxSpeed = stats.MaxSprintSpeed;
            acceleration = stats.SprintAcceleration;
        }
        else
        {
            maxSpeed = stats.MaxWalkSpeed;
            acceleration = stats.WalkAcceleration;
        }
    }
    
    /// <summary>
    /// Limits the player's horizontal/flat velocity (velocity in x and z axis)
    /// </summary>
    private void LimitVelocity()
    {
        RaycastHit hit = player.slopeSensor.hit;
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, hit.normal);
        
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            Vector3 verticalVel = rb.linearVelocity - flatVel;
            rb.linearVelocity = limitedVel + verticalVel;
        }
    }

    /// <summary>
    /// If the player's ground check is not on the ground but the slope cast is on the ground, apply a downward force to stick the player to the slope
    /// </summary>
    private void StickToSlope()
    {
        if (!player.groundSensor.grounded)
        {
            Debug.Log("Stick to slope");
            rb.AddForce(Vector3.down * stats.StickToSlopeForce);
        }
    }

}
