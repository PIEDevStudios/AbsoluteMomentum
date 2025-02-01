using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborne : State
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    private PlayerStats stats => player.stats;
    private float hardMaxSpeed, softMaxSpeed, acceleration;
    private Vector3 speedOnEnter; // Player's flat (x and z) speed when they enter airborne
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.SetTrigger("Jump");
        Debug.Log("Airborne");
        rb.linearDamping = stats.AirDrag;
        
        
        // Set max speed and acceleration
        
        speedOnEnter = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        acceleration = stats.SprintAcceleration * stats.AirAcceleration;
        hardMaxSpeed = speedOnEnter.magnitude;
        softMaxSpeed = player.stats.AirSoftMaxSpeed;
    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
        LimitVelocity();
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        rb.AddForce((orientation.forward * playerInput.moveVector.y + orientation.right * playerInput.moveVector.x).normalized * acceleration * 100f);
        NoInputDeceleration();
    }

    /// <summary>
    /// Limits the player's velocity
    /// If the player's velocity goes above the hardMaxSpeed limit, set speed to hardMaxSpeed
    /// If the player's velocity drops below the hardMaxSpeed limit and hardMaxSpeed > softMaxSpeed, set new hardMaxSpeed limit to current velo
    /// If the player's velocity drops below the hardMaxSpeed limit and currentVelo <= softMaxSpeed, set new hardMaxSpeed limit to softMaxSpeed 
    /// </summary>
    private void LimitVelocity()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > hardMaxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * hardMaxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        if (flatVel.magnitude < hardMaxSpeed && flatVel.magnitude > softMaxSpeed)
        {
            hardMaxSpeed = flatVel.magnitude;
        }
        else if (flatVel.magnitude < softMaxSpeed)
        {
            hardMaxSpeed = softMaxSpeed;
        }
        
        
        // Clamp Fall speed
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -stats.FallSpeedLimit, stats.FallSpeedLimit), rb.linearVelocity.z);
    }

    private void NoInputDeceleration()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // If player is not pressing any move button, decelerate them
        if (playerInput.moveVector.magnitude == 0f)
        {
            Debug.DrawRay(player.transform.position, -flatVel.normalized, Color.blue);
            rb.AddForce(-flatVel.normalized * stats.NoInputDeceleration);
        }
        // If our velocity is close to 0 and still not pressing an input, set velo to 0
        if (playerInput.moveVector.magnitude == 0f && flatVel.magnitude < 2f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}
