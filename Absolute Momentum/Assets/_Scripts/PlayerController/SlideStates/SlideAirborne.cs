using UnityEngine;

public class SlideAirborne : State
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    private PlayerStats stats => player.stats;
    private float hardMaxSpeed, softMaxSpeed, acceleration;
    private Vector3 speedOnEnter; // Player's flat (x and z) speed when they enter airborne
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        rb.linearDamping = stats.AirDrag;
        speedOnEnter = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        hardMaxSpeed = speedOnEnter.magnitude;
        acceleration = stats.AirAcceleration;
        player.playerSpeedManager.currentCurve = stats.airDragCurve;
    }
    
    
    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }
    
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        // Player Turning
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 playerInputVector = orientation.forward * player.playerInput.moveVector.y + orientation.right * player.playerInput.moveVector.x;
        Vector3 forceVector = playerInputVector.normalized * acceleration;
        float forceInVeloDirection = Vector3.Dot(forceVector, flatVel.normalized);
        Vector3 perpendicularForce = forceVector - (forceInVeloDirection * flatVel.normalized);
        
        rb.AddForce(perpendicularForce, ForceMode.Force);

        if (forceInVeloDirection < 0f)
        {
            rb.AddForce(forceInVeloDirection * flatVel.normalized, ForceMode.Force);
        }
        
        NoInputDeceleration();
        LimitVelocity();
    }
    
    
    /// <summary>
    /// Limits the player's velocity
    /// If the player's velocity goes above the hardMaxSpeed limit, set speed to hardMaxSpeed
    /// If the player's velocity drops below the hardMaxSpeed limit and hardMaxSpeed > softMaxSpeed, set new hardMaxSpeed limit to current velo
    /// If the player's velocity drops below the hardMaxSpeed limit and currentVelo <= softMaxSpeed, set new hardMaxSpeed limit to softMaxSpeed 
    /// </summary>
    private void LimitVelocity()
    {
        // Clamp Fall speed
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -stats.FallSpeedLimit, stats.FallSpeedLimit), rb.linearVelocity.z);
    }
    
    private void NoInputDeceleration()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // If player is not pressing any move button, decelerate them
        if (player.playerInput.moveVector.magnitude == 0f)
        {
            Debug.DrawRay(player.transform.position, -flatVel.normalized, Color.blue);
            rb.AddForce(-flatVel.normalized * stats.AirNoInputDeceleration);
        }
        // If our velocity is close to 0 and still not pressing an input, set velo to 0
        if (player.playerInput.moveVector.magnitude == 0f && flatVel.magnitude < 2f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}
