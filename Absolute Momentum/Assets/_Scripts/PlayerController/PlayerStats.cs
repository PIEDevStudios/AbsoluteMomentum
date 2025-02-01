using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Stats�")]
public class PlayerStats : ScriptableObject
{
    [field:Header("Ground Movement")]
    public float MaxWalkSpeed;
    public float MaxSprintSpeed;
    public float WalkAcceleration;
    public float SprintAcceleration;
    public float GroundDrag;
    public float NoInputDeceleration;
    public float StickToSlopeForce;
    
    [field:Header("Air Movement")]
    public float AirAcceleration;
    public float AirDrag;
    public float AirSoftMaxSpeed;

    [field: Header("Slide Movement")]
    public float minimumSlideSpeed; // minimum speed required to enter slide state (grounded)

    [FormerlySerializedAs("minimumSlideAirTime")] public float MinimumSlideAirTime; // minimum airtime required to enter slide state (airborne)
    [FormerlySerializedAs("SlideStopForce")] [FormerlySerializedAs("slideStopForce")] public float SlideDrag;
    [FormerlySerializedAs("slideBoostMultiplier")] public float SlideBoostMultiplier;
    [FormerlySerializedAs("slideGroundAcceleration")] public float SlideGroundAcceleration;
    public float SlideAirAcceleration;
    public Vector3 slidePlayerScale;

    [field: Header("Gravity")] 
    public bool gravityEnabled;

    public float NormalGravity;
    public float FallingGravityMultiplier;

    [field: ReadOnly, SerializeField] public float CurrentGravity;
    
    [field:Header("Jumping")]
    public float JumpForce;
    [FormerlySerializedAs("EndJumpEarlyForce")] public float EndJumpEarlyForceScale;
    public float EndJumpEarlyTime;
    public uint JumpFrameBufferAmount;
    public float FallSpeedLimit;
    
    
}