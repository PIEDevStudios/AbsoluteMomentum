using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpManager : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Player player;
    [SerializeField] private PlayerInput playerInput;
    private PlayerStats playerStats => player.stats;
    float FrameBufferNum => playerStats.JumpFrameBufferAmount;
    float JumpForce => playerStats.JumpForce;
    private float downwardForce => playerStats.EndJumpEarlyForceScale;

    private int framesSinceLastSpacebar, framesSinceOnGround;
    private bool jumping;

    private void Awake()
    {
        framesSinceLastSpacebar = (int)FrameBufferNum; // ensure player doesn't jump on start
        framesSinceOnGround = (int)FrameBufferNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.jumpPressedThisFrame)
        {
            framesSinceLastSpacebar = 0;
        }
        
        if((player.groundSensor.grounded || player.slopeSensor.isOnSlope) && rb.linearVelocity.y < 0)
        {
            jumping = false;
        }
        
        //creates variable jump, adds downward force if player lets go of space making character fall faster leading to smaller jump
        if (jumping && playerInput.jumpReleasedThisFrame && framesSinceOnGround <= playerStats.EndJumpEarlyTime && player.stateMachine.currentState == player.airborne)
        {
            rb.AddForce(Vector3.down * Mathf.Abs(rb.linearVelocity.y) * downwardForce, ForceMode.Impulse);
        }

    }

    void FixedUpdate()
    {
        if (framesSinceOnGround == -1)
        {
            framesSinceOnGround = (int)FrameBufferNum;
        }
        else if (player.stateMachine.currentState is not PlayerAirborne && (player.groundSensor.grounded || player.slopeSensor.isOnSlope))
        {
            framesSinceOnGround = 0;
        }
        else
        {
            framesSinceOnGround++;
        }
        framesSinceLastSpacebar++;
        if (framesSinceLastSpacebar < FrameBufferNum)
        {
            AttemptJump();
        }
        
        
    }

    void AttemptJump()
    {
        Debug.Log("Attempt Jump");
        
        // Only allow the player to jump in these states
        if (!(player.stateMachine.currentState is PlayerMove || player.stateMachine.currentState is PlayerIdle || player.stateMachine.currentState is PlayerSlide)) return;

        if (framesSinceOnGround < FrameBufferNum)
        {


            if(player.groundSensor.grounded || player.slopeSensor.isOnSlope)
            {
                if (player.stateMachine.currentState != player.slide)
                {
                    player.stateMachine.SetState(player.airborne);
                }
                
                
                Debug.Log("Jump");
                
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * playerStats.JumpForce, ForceMode.Impulse);
            }
            
           

            jumping = true;

            framesSinceLastSpacebar = (int)FrameBufferNum; // ensure two jumps don't happen off one input
            framesSinceOnGround = -1; // magic� (look at fixed update)
        }
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, $"Ticks since: Spacebar: {framesSinceLastSpacebar} Ground: {framesSinceOnGround}", style);
        }
        #endif
    }
}
