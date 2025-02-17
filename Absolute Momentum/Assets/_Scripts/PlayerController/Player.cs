using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : StateMachineCore
{
    
    // Variables that hold what states the player can be in
    [field: HorizontalLine(color: EColor.Gray)]
    [field: Header("States")]
    [field: SerializeField] public PlayerIdle idle { get; private set; }
    [field: SerializeField] public PlayerMove move { get; private set; }
    [field: SerializeField] public PlayerAirborne airborne { get; private set; }
    [field: SerializeField] public PlayerSlide slide { get; private set; }
    [field: SerializeField] public PlayerWallrun wallrun { get; private set; }

    // Sensor scripts used for ground checks and wall checks
    [field:HorizontalLine(color: EColor.Gray)]
    [field:Header("Sensors")]
    [field:SerializeField] public GroundSensor groundSensor {get; private set;}
    [field:SerializeField] public WallSensor wallSensor {get; private set;}
    [field:SerializeField] public SlopeSensor slopeSensor {get; private set;}

    
    // References to other components on the player
    [field:HorizontalLine(color: EColor.Gray)]
    [Header("Player Components")]
    [field:SerializeField] public Transform graphics { get; private set; }
    [field:SerializeField] public Transform playerObj { get; private set; }
    [field:SerializeField] public Transform orientation { get; private set; }
    [Expandable]
    [SerializeField] public PlayerStats stats;
    [field:SerializeField] public PlayerInput playerInput {get; private set;}
    [field:SerializeField] public CapsuleCollider playerCollider {get; private set;}
    [SerializeField] private PlayerJumpManager jumpManager;
    [field:SerializeField] public PlayerSpeedManager playerSpeedManager {get; private set;}

    [ReadOnly] public float wallrunResetTimer;
    [ReadOnly] public bool leavingGround;
    
    // Variables used for debugging
    [Header("Debug")] 
    [SerializeField] private Vector3 spawnPos;
    
    private float timeSinceLastGrounded;
    

    #region Unity Methods
    void Awake()
    {
        SetupInstances();
        ResetPlayer();
        
        // Disable gravity and simulate gravity manually (to allow for different gravity scales)
        rb.useGravity = false;
        stats.gravityEnabled = true;
        ChangeGravity(stats.NormalGravity);
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // Calls update logic in the currently active state
        stateMachine.currentState.DoUpdateBranch();
        wallrunResetTimer -= Time.deltaTime;
        
        // Debug reset player input
        if (playerInput.ResetInput)
        {
            ResetPlayer();
        }
        
        timeSinceLastGrounded += Time.deltaTime;

        if (groundSensor.grounded)
        {
            timeSinceLastGrounded = 0;
        }


        if (!groundSensor.grounded)
        {
            leavingGround = false;
        }
        
        // State transitions
        HandleTransitions();
        
    }
    
    void FixedUpdate() 
    {
        if (rb.linearVelocity.y > 0)
        {
            // Simulate custom gravity
            rb.AddForce(Vector3.down * stats.CurrentGravity, ForceMode.Force);
        }
        else
        {
            rb.AddForce(Vector3.down * stats.CurrentGravity * stats.FallingGravityMultiplier, ForceMode.Force);
        }

        
        // Call FixedUpdate logic
        stateMachine.currentState.DoFixedUpdateBranch(); 
    }

    #endregion
    
    #region Helper (Private) Methods
    /// <summary>
    /// Handles transitions between states in player state machine
    /// </summary>
    private void HandleTransitions()
    {
        
        // Cache xInput and yInput from playerInput script
        float xInput = playerInput.moveVector.x;
        float yInput = playerInput.moveVector.y;
        
        
        // condition for transitioning to a "grounded" state (move or idle) when transitioning from airborne
        bool airborneGroundCheck = stateMachine.currentState == airborne && !leavingGround;
        
        // condition for transitioning to a "grounded" state (move or idle) when transitioning from any state besides airborne
        bool nonAirborneGroundCheck = stateMachine.currentState == idle || stateMachine.currentState == move;
        
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, slopeSensor.hit.normal);
        
        // Transition to slide
        bool groundedSlideTransition = playerInput.slidePressedThisFrame && groundSensor.grounded && flatVel.magnitude > stats.minimumSlideSpeed;
        bool airborneSlideTransition = playerInput.slideHeld && !groundSensor.grounded && timeSinceLastGrounded > stats.MinimumSlideAirTime;
        if (groundedSlideTransition || airborneSlideTransition)
        {
            stateMachine.SetState(slide);
            return;
        }
        
        // Transition to wallrun
        if (!wallSensor.minHeightSensor.grounded && (wallSensor.wallLeft || wallSensor.wallRight) && playerInput.moveVector.y > 0 && stateMachine.currentState != slide && wallrunResetTimer < 0f)
        {
            stateMachine.SetState(wallrun);
            return;
        }
        
        // Transition to airborne
        if (!groundSensor.grounded && !slopeSensor.isOnSlope && ((stateMachine.currentState != slide && stateMachine.currentState != wallrun) || stateMachine.currentState.isComplete))
        {
            stateMachine.SetState(airborne);
            return;
        }
        
        // Transition to move
        if (groundSensor.grounded && (xInput != 0 || yInput != 0) && (nonAirborneGroundCheck || airborneGroundCheck || stateMachine.currentState.isComplete))
        {
            stateMachine.SetState(move);
            return;
        }

        float timeSinceLastMove = Time.time - playerInput.timeOfLastMoveInput;
        
        // Transition to idle
        if (groundSensor.grounded && timeSinceLastMove >= 0.1f && (nonAirborneGroundCheck || airborneGroundCheck || stateMachine.currentState.isComplete))
        {
            stateMachine.SetState(idle);
            return;
        }


        
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Changes the custom gravity scale that the player is currently experiencing
    /// </summary>
    /// <param name="gravity"></param>
    public void ChangeGravity(float gravity)
    {
        stats.CurrentGravity = gravity;
    }

    /// <summary>
    /// Reset all animation triggers. If a new trigger is added to the animator, it needs to be reset in this function.
    /// </summary>
    public void ResetAllTriggers()
    {
        // animator.ResetTrigger("Jump");
        // animator.ResetTrigger("Walk");
        // animator.ResetTrigger("Roll");
        // animator.ResetTrigger("Attack");
        // animator.ResetTrigger("Idle");
    }

    /// <summary>
    /// Set a trigger in the player's animator
    /// </summary>
    /// <param name="trigger"></param>
    public void SetTrigger(string trigger)
    {
        // ResetAllTriggers();
        // animator.SetTrigger(trigger);
    }

    #endregion
    
    #region Debug Methods
    /// <summary>
    /// DEBUG METHOD. Resets the player's position and health
    /// </summary>
    private void ResetPlayer()
    {
        stateMachine.SetState(idle);
        rb.linearVelocity = Vector3.zero;
        rb.transform.position = spawnPos;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.25f, rb.linearVelocity + "\n" + flatVel.magnitude, style);
        
        }
#endif
    }
    #endregion
    
}