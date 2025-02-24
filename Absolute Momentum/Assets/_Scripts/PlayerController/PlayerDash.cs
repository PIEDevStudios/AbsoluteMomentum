using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDash : State
{
    
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    
    [SerializeField] private GameObject camera;
    private Vector3 lastVelocity;
    [NonSerialized] public float timeInDash;
    private PlayerStats stats => player.stats;
    private PlayerInput playerInput => player.playerInput;

    private Vector3 lastMovementVector;
    private float DashSpeed;
    
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        player.ChangeGravity(0);
        lastVelocity = player.rb.linearVelocity;
        timeInDash = 0;

        if (stats.DashImplementation == PlayerStats.DashImpl.UseOrientation)
        {
            lastMovementVector = (orientation.forward * playerInput.moveVector.y + orientation.right * playerInput.moveVector.x).normalized;
            if (lastMovementVector == Vector3.zero)
            {
                lastMovementVector = orientation.forward;
            }
        }
        if (stats.DashImplementation == PlayerStats.DashImpl.UseHorizontalVelocity)
        {
            lastMovementVector = Vector3.ProjectOnPlane(player.rb.linearVelocity, Vector3.up).normalized;
            if (lastMovementVector == Vector3.zero)
            {
                lastMovementVector = orientation.forward;
            }
        }
        else if (stats.DashImplementation == PlayerStats.DashImpl.UseVelocity)
        {
            lastMovementVector = player.rb.linearVelocity.normalized;
            if (lastMovementVector == Vector3.zero)
            {
                lastMovementVector = orientation.forward;
            }
        }
        else if (stats.DashImplementation == PlayerStats.DashImpl.UseCamera)
        {
            lastMovementVector = camera.transform.forward;
        }
        
        if (stats.AddSpeedToDash == PlayerStats.DashSpeedAddImpl.AddAllSpeed)
        {
            DashSpeed = stats.DashSpeed + player.rb.linearVelocity.magnitude;
        }
        else if (stats.AddSpeedToDash == PlayerStats.DashSpeedAddImpl.AddHorizontalSpeed)
        {
            DashSpeed = stats.DashSpeed + Vector3.ProjectOnPlane(player.rb.linearVelocity, Vector3.up).magnitude;
        }
        else if (stats.AddSpeedToDash == PlayerStats.DashSpeedAddImpl.AddOrientedSpeed)
        {
            DashSpeed = stats.DashSpeed + Vector3.Dot(player.rb.linearVelocity, lastMovementVector);
        }
        else if (stats.AddSpeedToDash == PlayerStats.DashSpeedAddImpl.AddHorizontalOrientedSpeed)
        {
            DashSpeed = stats.DashSpeed + Vector3.Dot(Vector3.ProjectOnPlane(player.rb.linearVelocity, Vector3.up), lastMovementVector);
        }
        else
        {
            DashSpeed = stats.DashSpeed;
        }

        SetDashVelocity();
    }


    private void SetDashVelocity()
    {
        if (!(stats.VelocityChangeImplementation == PlayerStats.VelocityChangeImpl.None))
        {
            Vector3 dashDir;
            if (stats.VelocityChangeImplementation == PlayerStats.VelocityChangeImpl.UseOrientation)
            {
                dashDir = (orientation.forward * playerInput.moveVector.y + orientation.right * playerInput.moveVector.x).normalized;
            }
            else
            {
                dashDir = (camera.transform.forward * playerInput.moveVector.y + camera.transform.right * playerInput.moveVector.x).normalized;   
            }

            if (dashDir == Vector3.zero)
            {
                dashDir = lastMovementVector;
            }
        
            dashDir = Vector3.Slerp(lastMovementVector, dashDir, stats.velocityChangeMidDashStrength).normalized;
            player.rb.linearVelocity = dashDir * stats.DashSpeed;

            lastMovementVector = dashDir;
        }
        else
        {
            player.rb.linearVelocity = lastMovementVector * DashSpeed;
        }
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.ChangeGravity(stats.NormalGravity);
        Vector3 Direction;
        if (stats.ReorientVelocity)
        {
            Direction = lastMovementVector;
        }
        else
        {
            Direction = lastVelocity.normalized;
        }
        
        if (stats.AddSpeedFromDash == PlayerStats.DashSpeedFromImpl.SetDashSpeed)
        {
            player.rb.linearVelocity = DashSpeed * Direction;
        }
        else if (stats.AddSpeedFromDash == PlayerStats.DashSpeedFromImpl.AddDashSpeed)
        {
            player.rb.linearVelocity = (lastVelocity.magnitude + stats.DashSpeed) * Direction;
        }
        else
        {
            player.rb.linearVelocity = lastVelocity.magnitude * Direction;
        }
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        timeInDash += Time.deltaTime;
        SetDashVelocity();
        
        if (timeInDash >= stats.DashTime)
        {
            isComplete = true;
        }
    }
    }