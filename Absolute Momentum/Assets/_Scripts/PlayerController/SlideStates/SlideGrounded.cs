using Unity.Services.Authentication.PlayerAccounts;
using Unity.VisualScripting;
using UnityEngine;

public class SlideGrounded : State
{
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    private Vector3 directionCross;
    private PlayerStats stats => player.stats;
    private float hardMaxSpeed, softMaxSpeed;
    private bool skipFirstLimitCall;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.playerSpeedManager.currentCurve = stats.slideDragCurve;
        skipFirstLimitCall = true;
        
        rb.linearDamping = player.stats.SlideDrag;
        player.ChangeGravity(0f);
        
        // If the player enters grounded on a slope
        if (player.slopeSensor.isOnSlope && player.groundSensor.grounded && rb.linearVelocity.y < 0f)
        {
            SlopeBoost();
        }
        
        // Add force down to stick the player to the ground
        // rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        

    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, player.slopeSensor.hit.normal);
        player.playerObj.forward = flatVel;
        if (player.slopeSensor.isOnSlope && player.groundSensor.grounded && rb.linearVelocity.y < 0f)
        {
            directionCross = new Vector3(-player.slopeSensor.hit.normal.z, 0, player.slopeSensor.hit.normal.x).normalized;
            Vector3 direction = Vector3.Cross(player.slopeSensor.hit.normal, directionCross).normalized;
            rb.AddForce(direction * (player.stats.SlopeSlideForce), ForceMode.Force);
        }
        
        // Transition out of state
        if (flatVel.magnitude < player.stats.minimumSlideSpeed)
        {
            player.stateMachine.SetState(player.move);
            return;
        }
        
        if (player.slopeSensor.isOnSlope)
        {
            rb.linearDamping = 0f;
        }
        else
        {
            rb.linearDamping = player.stats.SlideDrag;
        }
        
        // Player Turning
        
        RaycastHit hit = player.slopeSensor.hit;
        Vector3 forwardOriented = Vector3.Cross(orientation.right, hit.normal).normalized;
        Vector3 rightOriented = Vector3.Cross(hit.normal, forwardOriented).normalized;
        Vector3 playerInputVector = forwardOriented * player.playerInput.moveVector.y + rightOriented * player.playerInput.moveVector.x;
        Vector3 forceVector = playerInputVector.normalized * (player.stats.SlideGroundAcceleration * (1 / flatVel.magnitude));
        Vector3 forceInVeloDirection = Vector3.Dot(forceVector, flatVel.normalized) * flatVel.normalized;
        Vector3 perpendicularForce = forceVector - forceInVeloDirection;
        Debug.Log("Flat Vel: " + flatVel);
        Debug.Log("Corrected Force Vector: " + perpendicularForce);
        rb.AddForce(perpendicularForce, ForceMode.Force);
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
        if (player.slopeSensor.isOnSlope)
        {
            rb.AddForce(-player.slopeSensor.hit.normal * player.stats.StickToSlopeForce, ForceMode.Force);
        }
        
    }
    private void SlopeBoost()
    {
        // Calculate direction of the vector we need to cross the normal with depending on which way the slope is pointed
        directionCross = new Vector3(-player.slopeSensor.hit.normal.z, 0, player.slopeSensor.hit.normal.x).normalized;
        Vector3 direction = Vector3.Cross(player.slopeSensor.hit.normal, directionCross);
        // float slopeAngleFactor = 1 / player.slopeSensor.hit.normal.y;
        float ySpeedFactor = Mathf.Abs(Vector3.ProjectOnPlane(rb.linearVelocity, player.slopeSensor.hit.normal).y);
        // Debug.Log("Y Velo factor: " + ySpeedFactor * player.stats.BoostYVeloMult);
        rb.AddForce(direction.normalized * (player.stats.BoostYVeloMult * ySpeedFactor), ForceMode.Impulse);
    }
    
    
}
