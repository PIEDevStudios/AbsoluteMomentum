using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpManager : NetworkBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Player player;
    [SerializeField] private PlayerInput playerInput;
    private PlayerStats playerStats => player.stats;
    float JumpBufferTime => playerStats.JumpBufferAmount;
    float JumpForce => playerStats.JumpForce;
    private float downwardForce => playerStats.EndJumpEarlyForceScale;

    private float timeSinceLastSpacebar, timeSinceOnGround, coyoteTimer;
    private bool jumping;
    private bool jumpEnded;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        timeSinceLastSpacebar = JumpBufferTime; // ensure player doesn't jump on start
        timeSinceOnGround = JumpBufferTime;
    }

    public void Update()
    {
        if (!IsOwner) return;

        // From update
        if (player.playerInput.jumpPressedThisFrame)
        {
            timeSinceLastSpacebar = 0;
        }
        
        if((player.groundSensor.grounded || player.slopeSensor.isOnSlope) && rb.linearVelocity.y < 0)
        {
            jumping = false;
        }
        
        //
        // if (framesSinceOnGround == -1)
        // {
        //     framesSinceOnGround = (int)FrameBufferNum;
        // }
        
        if (player.stateMachine.currentState is not PlayerAirborne && (player.groundSensor.grounded || player.slopeSensor.isOnSlope))
        {
            timeSinceOnGround = 0;
        }
        else
        {
            timeSinceOnGround += Time.deltaTime;
        }
        timeSinceLastSpacebar+= Time.deltaTime;
        if (timeSinceLastSpacebar < JumpBufferTime)
        {
            AttemptJump();
        }
        
        
        //creates variable jump, adds downward force if player lets go of space making character fall faster leading to smaller jump
        if (jumping && timeSinceOnGround > -1 && !jumpEnded && !playerInput.jumpHeld && timeSinceOnGround <= playerStats.EndJumpEarlyTime && player.stateMachine.currentState == player.airborne)
        {
            Debug.Log("end jump early" + rb.linearVelocity.y);
            Debug.Log("Frames since on ground: " + timeSinceOnGround);
            jumpEnded = true;
            rb.AddForce(Vector3.down * Mathf.Abs(rb.linearVelocity.y) * downwardForce * Time.deltaTime * 100, ForceMode.Impulse);
        }
    }

    void AttemptJump()
    {
        // If its been too long since the last jump input, return
        if (timeSinceOnGround >= JumpBufferTime && !jumping)
        {
            player.leavingGround = false;
            return;
        }
        
        // Only allow the player to jump in these states
        if (!(player.stateMachine.currentState is PlayerMove || player.stateMachine.currentState is PlayerIdle || player.stateMachine.currentState is PlayerSlide)) return;



        if(player.groundSensor.grounded || player.slopeSensor.isOnSlope)
        {
            if (player.stateMachine.currentState != player.slide)
            {
                player.stateMachine.SetState(player.airborne);
            }
            
            
            Debug.Log("Jump: " + timeSinceLastSpacebar);
            
            // If we buffer our jump, reset our y velocity before the jump
            if (timeSinceLastSpacebar != 1)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
            jumpEnded = false;
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
            rb.AddForce(Vector3.up * playerStats.JumpForce, ForceMode.Impulse);
            player.leavingGround = true;
        }
        
        jumping = true;

        timeSinceLastSpacebar = JumpBufferTime; // ensure two jumps don't happen off one input
        timeSinceOnGround = 0; // magic� (look at fixed update)
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, $"Ticks since: Spacebar: {timeSinceLastSpacebar} Ground: {timeSinceOnGround}", style);
        }
        #endif
    }
}
