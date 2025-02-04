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
    private Vector3 speedOnEnter; // Player's flat (x and z) speed when they enter airborne
    private bool skipFirstLimitCall;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        
        speedOnEnter = Vector3.zero;
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
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
        player.graphics.forward = flatVel;


        if (player.slopeSensor.isOnSlope)
        {
            rb.linearDamping = 0f;
        }
        else
        {
            rb.linearDamping = player.stats.SlideDrag;
        }
        
        // If we are moving too slow to maintain a slide, force player into the move state
        if (flatVel.magnitude < player.stats.minimumSlideSpeed)
        {
            player.stateMachine.SetState(player.move);
        }
        
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, player.slopeSensor.hit.normal);
        
        if (player.slopeSensor.isOnSlope && player.groundSensor.grounded && rb.linearVelocity.y < 0f)
        {
            directionCross = new Vector3(-player.slopeSensor.hit.normal.z, 0, player.slopeSensor.hit.normal.x).normalized;
            Vector3 direction = Vector3.Cross(player.slopeSensor.hit.normal, directionCross).normalized;
            rb.AddForce(direction * (player.stats.SlopeSlideForce), ForceMode.Force);
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
        LimitVelocity();
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
    
        
    /// <summary>
    /// Limits the player's velocity
    /// If the player's velocity goes above the hardMaxSpeed limit, set speed to hardMaxSpeed
    /// If the player's velocity drops below the hardMaxSpeed limit and hardMaxSpeed > softMaxSpeed, set new hardMaxSpeed limit to current velo
    /// If the player's velocity drops below the hardMaxSpeed limit and currentVelo <= softMaxSpeed, set new hardMaxSpeed limit to softMaxSpeed 
    /// </summary>
    private void LimitVelocity()
    {
        // Skips first limit call to wait for boost force to kick in
        if (skipFirstLimitCall)
        {
            skipFirstLimitCall = false;
            return;
        }
        
        // set speedOnEnter once boost force has kicked in
        if (speedOnEnter == Vector3.zero)
        {
            // for speed limiting
            speedOnEnter = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            hardMaxSpeed = speedOnEnter.magnitude;
            softMaxSpeed = 0f;
            return;
        }
        
        
        
        // Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // if (flatVel.magnitude > hardMaxSpeed)
        // {
        //     Vector3 limitedVel = flatVel.normalized * hardMaxSpeed;
        //     rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        // }
        // flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        //
        // if (flatVel.magnitude < hardMaxSpeed && flatVel.magnitude > softMaxSpeed)
        // {
        //     hardMaxSpeed = flatVel.magnitude;
        // }
        // else if (flatVel.magnitude < softMaxSpeed)
        // {
        //     hardMaxSpeed = softMaxSpeed;
        // }
        
        
        // Clamp Fall speed
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -stats.FallSpeedLimit, stats.FallSpeedLimit), rb.linearVelocity.z);
    }
    
}
