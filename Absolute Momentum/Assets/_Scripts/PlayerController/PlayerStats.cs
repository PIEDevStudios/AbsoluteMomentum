using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Stats�")]
public class PlayerStats : ScriptableObject
{
    [Header("Ground Movement")]
    // public float MaxSprintSpeed;
    public float SprintAcceleration;
    public float GroundDrag;
    public float NoInputDeceleration;
    public float StickToSlopeForce;
    public AnimationCurve groundDragCurve;
    
    [Header("Air Movement")]
    public float AirAcceleration;
    public float AirDrag;
    // public float AirSoftMaxSpeed;
    public float AirStrafeAcceleration;
    public AnimationCurve airDragCurve;

    [Header("Slide Movement")]
    public float minimumSlideSpeed; // minimum speed required to enter slide state (grounded)

    public float MinimumSlideAirTime; // minimum airtime required to enter slide state (airborne)
    public float SlideDrag;
    public float BoostYVeloMult; 
    public float SlopeSlideForce; // amount that the slope pulls the player down 

    public float SlideGroundAcceleration;
    public float SlideAirAcceleration;
    public Vector3 slidePlayerScale;
    public AnimationCurve slideDragCurve;

    [Header("Wall Movement")]
    public float wallrunForce;

    public float stickToWallForce;
    public float wallrunResetTime;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public AnimationCurve wallDragCurve;

    [Header("Gravity")] 
    public bool gravityEnabled;

    public float NormalGravity;
    public float WallrunGravity;
    public float FallingGravityMultiplier;

    [ReadOnly, SerializeField] public float CurrentGravity;
    
    [Header("Jumping")]
    public float JumpForce;
    public float EndJumpEarlyForceScale;
    public float EndJumpEarlyTime;
    public uint JumpFrameBufferAmount;
    public float FallSpeedLimit;

    [Header("Dash")]
    public float DashSpeed;
    public float DashTime;
    public float velocityChangeMidDashStrength;
    public enum DashImpl
    {
        UseOrientation = 1,
        UseVelocity = 2,
        UseHorizontalVelocity = 3,
        UseCamera = 4,
    }
    public enum VelocityChangeImpl
    {
        None = 1,
        UseOrientation = 2,
        UseCamera = 3,
    }
    public enum DashSpeedAddImpl
    {
        None = 1,
        AddAllSpeed = 2,
        AddHorizontalSpeed = 3,
        AddOrientedSpeed = 4,
        AddHorizontalOrientedSpeed = 5,
    }

    public enum DashSpeedFromImpl
    {
        None = 1,
        AddDashSpeed = 2,
        SetDashSpeed = 3,
    }

    public DashSpeedAddImpl AddSpeedToDash;
    public DashImpl DashImplementation;
    public VelocityChangeImpl VelocityChangeImplementation;
    public bool ReorientVelocity;
    public DashSpeedFromImpl AddSpeedFromDash;
}