using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;
    private void Start()
    {
        player.stateMachine.OnStateChanged += OnStateChanged;
        animator = player.animator;
    }

    /// <summary>
    /// This method is called when a new state is entered
    /// </summary>
    private void OnStateChanged(object sender, StateMachine.OnStateChangedEventArgs eventArgs)
    {
        SetTrigger(getTriggerName(eventArgs.nextState));
    }

    private String getTriggerName(State state)
    {
        if (state == player.idle)
        {
            return "Idle";
        }
        else if (state == player.move)
        {
            return "Walk";
        }
        else if (state == player.airborne)
        {
            return "Jump";
        }

        return "";
    }
    
    /// <summary>
    /// Reset all animation triggers. If a new trigger is added to the animator, it needs to be reset in this function.
    /// </summary>
    public void ResetAllTriggers()
    {
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Walk");
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
    private void CheckForSprint()
    {
        if (!player.playerInput.sprintHeld)
        {
            animator.SetBool("Sprint", true);
            // footstepEmitter.EventInstance.setParameterByName("Sprinting", 1.0f);
        }
        else
        {
            animator.SetBool("Sprint", false);
            // footstepEmitter.EventInstance.setParameterByName("Sprinting", 0.0f);
        }
    }
    
}
