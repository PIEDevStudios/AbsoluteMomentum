using Unity.VisualScripting;
using UnityEngine;

public class SlideGrounded : State
{
    [SerializeField] private Player player;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        // Add force down to stick the player to the ground
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        rb.linearDamping = 0f;
        player.ChangeGravity(0f);

    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        
        // Slide stop force
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, player.slopeSensor.hit.normal);
        rb.AddForce(-flatVel * player.stats.slideStopForce, ForceMode.Force);
        
        StickToSlope();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.ChangeGravity(player.stats.NormalGravity);
    }
    
    /// <summary>
    /// If the player's ground check is not on the ground but the slope cast is on the ground, apply a downward force to stick the player to the slope
    /// </summary>
    private void StickToSlope()
    {
        if (!player.groundSensor.grounded)
        {
            Debug.Log("Stick to slope");
            rb.AddForce(Vector3.down * player.stats.StickToSlopeForce, ForceMode.Force);
        }
    }
    
}
