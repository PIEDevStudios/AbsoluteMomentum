using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Stats�")]
public class PlayerStats : ScriptableObject
{
    [field:Header("Ground Movement")]
    [field:SerializeField] public float MaxWalkSpeed {get; private set;}
    [field:SerializeField] public float MaxSprintSpeed {get; private set;}
    [field:SerializeField] public float WalkAcceleration {get; private set;}
    [field:SerializeField] public float SprintAcceleration {get; private set;}
    [field:SerializeField] public float GroundDrag {get; private set;}
    [field:SerializeField] public float NoInputDeceleration {get; private set;}
    
    [field:Header("Air Movement")]
    [field:SerializeField] public float AirAccelerationMultiplier {get; private set;}
    [field:SerializeField] public float AirDrag {get; private set;}

    [field: Header("Gravity")] 
    [field: SerializeField] public bool gravityEnabled;
    [field:SerializeField] public float NormalGravity {get; private set;}
    [field:SerializeField] public float GroundGravity {get; private set;}
    
    [field:ReadOnly, SerializeField] public float CurrentGravity {get; set;}
    
    [field:Header("Jumping")]
    [field:SerializeField] public float JumpForce {get; private set;}
    [field:SerializeField] public float EndJumpEarlyForce {get; private set;}
    [field:SerializeField] public uint JumpFrameBufferAmount {get; private set;}
    [field:SerializeField] public float FallSpeedLimit {get; private set;}
    
    
}