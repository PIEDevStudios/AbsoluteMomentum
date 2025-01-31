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

    [field: Header("Gravity")] 
    public bool gravityEnabled;

    public float NormalGravity;

    [field: ReadOnly, SerializeField] public float CurrentGravity;
    
    [field:Header("Jumping")]
    public float JumpForce;
    public float EndJumpEarlyForce;
    public uint JumpFrameBufferAmount;
    public float FallSpeedLimit;
    
    
}