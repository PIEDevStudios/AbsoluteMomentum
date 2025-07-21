using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerSlide : State
{
    [SerializeField] private Player player;
    [field: SerializeField] public SlideGrounded grounded { get; private set; }
    [field: SerializeField] public SlideAirborne airborne { get; private set; }
    [SerializeField] private Transform orientation, colliderPivot, graphics;
    private Vector3 enterHitboxScale;
 
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        
        enterHitboxScale = colliderPivot.localScale;
        colliderPivot.localScale = player.stats.slidePlayerScale;
        graphics.localScale = player.stats.slidePlayerScale;
        
        
        if (!player.groundSensor.IsGroundedCoyote && !player.slopeSensor.isOnSlope)
        {
            stateMachine.SetState(airborne, true);
        }
        
        if (player.groundSensor.IsGroundedCoyote)
        {
            stateMachine.SetState(grounded, true);
        }

    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
        
        if (player.playerInput.slideReleasedThisFrame)
        {
            isComplete = true;
        }
        
        HandleTransitions();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        colliderPivot.localScale = enterHitboxScale;
        graphics.localScale = enterHitboxScale;
    }


    /// <summary>
    /// Handles transitions between states in slide state machine
    /// </summary>
    private void HandleTransitions()
    {
        
        if (!player.groundSensor.grounded && !player.slopeSensor.isOnSlope)
        {
            stateMachine.SetState(airborne);
        }
        
        if (player.groundSensor.grounded)
        {
            stateMachine.SetState(grounded);
        }
    }
    
}
