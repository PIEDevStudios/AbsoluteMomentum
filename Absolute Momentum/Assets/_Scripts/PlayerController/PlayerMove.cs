using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using FMODUnity;

public class PlayerMove : State
{
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
        player.playerSpeedManager.currentCurve = stats.groundDragCurve;
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
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        
    }

    public override void DoTickUpdateState(PlayerInput.InputValues inputValues)
    {
        base.DoTickUpdateState(inputValues);
        RaycastHit hit = player.slopeSensor.hit;
        Vector3 forwardOriented = Vector3.Cross(orientation.right, hit.normal).normalized;
        Vector3 rightOriented = Vector3.Cross(hit.normal, forwardOriented).normalized;
        // Adds a force to the player in the direction they are pressing relative to the camera
        //Debug.Log("MOVE FIXED UPDATE");
        rb.AddForce((forwardOriented * inputValues.moveVector.y + rightOriented * inputValues.moveVector.x).normalized * (stats.SprintAcceleration * 100f));
        NoInputDeceleration(inputValues);
        StickToSlope();
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
    
    private void NoInputDeceleration(PlayerInput.InputValues inputValues)
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // If player is not pressing any move button, decelerate them
        if (inputValues.moveVector.magnitude == 0f)
        {
            Debug.DrawRay(player.transform.position, -flatVel.normalized, Color.blue);
            rb.AddForce(-flatVel.normalized * stats.GroundNoInputDeceleration);
        }
        // If our velocity is close to 0 and still not pressing an input, set velo to 0
        if (inputValues.moveVector.magnitude == 0f && flatVel.magnitude < 2f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

}
