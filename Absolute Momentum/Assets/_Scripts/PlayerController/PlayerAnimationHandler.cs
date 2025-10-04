using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationHandler : NetworkBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private WallSensor wallSensor;
    private Animator animator;
    public override void OnNetworkSpawn()
    {
        animator = player.animator;
        if (IsOwner)
        {
            player.stateMachine.OnStateChanged += OnStateChanged;
            player.slide.stateMachine.OnStateChanged += OnStateChanged;
            player.move.stateMachine.OnStateChanged += OnStateChanged;
        }
    }

    public void Update()
    {
        if (!IsOwner) return;
        SetWallLeft(wallSensor.wallLeft);
        
        animator.SetBool("Grounded", player.groundSensor.grounded);
        if (player.stateMachine.currentState == player.idle)
        {
            animator.SetBool("Moving", false);
        }
        else
        {
            animator.SetBool("Moving", true);
        }
    }

    public void SetWallLeft(bool wallLeft)
    {
        animator.SetBool("Wall Left", wallLeft);
    }

    /// <summary>
    /// This method is called when a new state is entered
    /// </summary>
    private void OnStateChanged(object sender, StateMachine.OnStateChangedEventArgs eventArgs)
    {
        if (!IsOwner) return;

        if (eventArgs.nextState == player.move) return;
        
        if (eventArgs.nextState == player.slide || eventArgs.nextState == player.slide.grounded || eventArgs.nextState == player.slide.airborne)
        {
            animator.SetBool("Slide", true);

            if (eventArgs.nextState == player.slide) return;
        }
        else
        {
            animator.SetBool("Slide", false);
        }
        
        SetTrigger(getTriggerName(eventArgs.nextState));
        Debug.Log("Output Trigger: " + getTriggerName(eventArgs.nextState));
    }

    private String getTriggerName(State state)
    {
        Debug.Log("Transitioning to: " + state);
        if (state == player.idle)
        {
            return "Idle";
        }
        if (state == player.slide.grounded || state == player.move.runState)
        {
            return "Run";
        }
        if (state == player.airborne || state == player.slide.airborne)
        {
            return "Jump";
        }
        if (state == player.wallrun || state == player.wallSlide)
        {
            return "Wallrun";
        }

        if (state == player.move.crouchState)
        {
            return "Crouch Walk";
        }

        return "";
    }
    
    /// <summary>
    /// Reset all animation triggers. If a new trigger is added to the animator, it needs to be reset in this function.
    /// </summary>
    public void ResetAllTriggers()
    {
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Run");
        animator.ResetTrigger("Wallrun");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Crouch Walk");

    }

    /// <summary>
    /// Set a trigger in the player's animator
    /// </summary>
    /// <param name="trigger"></param>
    public void SetTrigger(string trigger)
    {
        ResetAllTriggers();
        animator.SetTrigger(trigger);
    }
    
    /// <summary>
    /// Check if player is sprinting
    /// </summary>
    // private void CheckForSprint()
    // {
    //     if (!player.playerInput.sprintHeld)
    //     {
    //         animator.SetBool("Sprint", true);
    //         // footstepEmitter.EventInstance.setParameterByName("Sprinting", 1.0f);
    //     }
    //     else
    //     {
    //         animator.SetBool("Sprint", false);
    //         // footstepEmitter.EventInstance.setParameterByName("Sprinting", 0.0f);
    //     }
    // }
    
}
