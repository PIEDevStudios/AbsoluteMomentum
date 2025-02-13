using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Stats�")]
public class PlayerStats : ScriptableObject
{
    [Header("Ground Movement")]
    public float MaxWalkSpeed;
    public float MaxSprintSpeed;
    public float WalkAcceleration;
    public float SprintAcceleration;
    public float GroundDrag;
    public float NoInputDeceleration;
    public float StickToSlopeForce;
    
    [Header("Air Movement")]
    public float AirAcceleration;
    public float AirDrag;
    public float AirSoftMaxSpeed;
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

    [Header("Wall Movement")]
    public float wallrunForce;

    public float wallrunResetTime;
    public float wallJumpUpForce;
    public float wallJumpSideForce;

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
    
    
}