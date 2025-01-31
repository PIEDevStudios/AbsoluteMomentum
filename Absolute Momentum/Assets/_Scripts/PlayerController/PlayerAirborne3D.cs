using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborne3D : State
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    private PlayerStats stats => player.stats;
    private float maxSpeed, acceleration;
    private Vector3 speedOnEnter; // Player's flat (x and z) speed when they enter airborne
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.SetTrigger("Jump");
        rb.linearDamping = stats.AirDrag;
        
        
        // Set max speed and acceleration
        
        speedOnEnter = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        acceleration = stats.SprintAcceleration * stats.AirAcceleration;
        maxSpeed = speedOnEnter.magnitude;
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        rb.AddForce((orientation.forward * playerInput.moveVector.y + orientation.right * playerInput.moveVector.x).normalized * acceleration * 100f);
        NoInputDeceleration();
        LimitVelocity();
    }

    private void LimitVelocity()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
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
