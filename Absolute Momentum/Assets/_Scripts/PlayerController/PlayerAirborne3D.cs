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
    private bool sprintOnEnter; // true if player was sprinting as they were entering the state
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.SetTrigger("Jump");
        rb.linearDamping = stats.AirDrag;
        SetMaxSpeed();
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

    /// <summary>
    /// Set the maxSpeed depending on if the player was sprinting before transitioning into airborne
    /// </summary>
    private void SetMaxSpeed()
    {
        sprintOnEnter = playerInput.sprintHeld;
        if (sprintOnEnter)
        {
            acceleration = stats.SprintAcceleration * stats.AirAccelerationMultiplier;
            maxSpeed = stats.MaxSprintSpeed;
        }
        else
        {
            acceleration = stats.WalkAcceleration * stats.AirAccelerationMultiplier;
            maxSpeed = stats.MaxWalkSpeed;
        }
    }
}
