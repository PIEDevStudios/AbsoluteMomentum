using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationHandler : NetworkBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;
    public override void OnNetworkSpawn()
    {
        animator = player.animator;
        if (IsOwner)
        {
            player.stateMachine.OnStateChanged += OnStateChanged;
        }
    }

    /// <summary>
    /// This method is called when a new state is entered
    /// </summary>
    private void OnStateChanged(object sender, StateMachine.OnStateChangedEventArgs eventArgs)
    {
        Debug.Log("State changed to : " + eventArgs.nextState);
        SetTrigger(getTriggerName(eventArgs.nextState));
    }

    private String getTriggerName(State state)
    {
        if (state == player.idle)
        {
            return "Idle";
        }
        if (state == player.move)
        {
            return "Run";
        }
        if (state == player.airborne)
        {
            return "Jump";
        }
        if (state == player.slide)
        {
            return "Slide";
        }
        if (state == player.wallrun)
        {
            return "Wallrun";
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
        animator.ResetTrigger("Roll");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Idle");
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
