using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using FMODUnity;

public class PlayerMove3D : State
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
        player.SetTrigger("Walk");
        rb.linearDamping = stats.GroundDrag;
        // footstepEmitter = AudioManager.Instance.InitializeEventEmitter(FMODEvents.Sounds.PlayerFootsteps_Grass, player.playerObj.gameObject);
        // footstepEmitter.Play();
        // player.ChangeGravity(stats.GroundGravity);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.animator.SetBool("Sprint", false);
        animator.Play("Walk");
        // footstepEmitter.Stop();
        
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
        if (playerInput.sprintHeld)
        {
            maxSpeed = stats.MaxSprintSpeed;
            acceleration = stats.SprintAcceleration;
            player.animator.SetBool("Sprint", true);
            
            // footstepEmitter.EventInstance.setParameterByName("Sprinting", 1.0f);
        }
        else
        {
            maxSpeed = stats.MaxWalkSpeed;
            acceleration = stats.WalkAcceleration;
            player.animator.SetBool("Sprint", false);
            
            // footstepEmitter.EventInstance.setParameterByName("Sprinting", 0.0f);
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
            rb.AddForce(Vector3.down * stats.GroundGravity);
        }
    }

}
