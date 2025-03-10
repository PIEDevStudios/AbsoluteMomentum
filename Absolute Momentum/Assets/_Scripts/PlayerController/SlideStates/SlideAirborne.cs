using UnityEngine;

public class SlideAirborne : State
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    private PlayerStats stats => player.stats;
    private float hardMaxSpeed, softMaxSpeed;
    private Vector3 speedOnEnter; // Player's flat (x and z) speed when they enter airborne
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.SetTrigger("Jump");
        rb.linearDamping = stats.AirDrag;
        speedOnEnter = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        hardMaxSpeed = speedOnEnter.magnitude;
        player.playerSpeedManager.currentCurve = stats.airDragCurve;
    }
    
    
    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }
    
    public override void DoTickUpdateState(PlayerInput.InputValues inputValues)
    {
        base.DoTickUpdateState(inputValues);
        // Player Turning
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        player.playerObj.forward = flatVel;
        Vector3 playerInputVector = orientation.forward * inputValues.moveVector.y + orientation.right * inputValues.moveVector.x;
        Vector3 forceVector = playerInputVector.normalized * (player.stats.SlideAirAcceleration * (1 / flatVel.magnitude));
        float forceInVeloDirection = Vector3.Dot(forceVector, flatVel.normalized);
        Vector3 perpendicularForce = forceVector - (forceInVeloDirection * flatVel.normalized);
        Debug.Log("Flat Vel: " + flatVel);
        Debug.Log("Corrected Force Vector: " + perpendicularForce);
        rb.AddForce(perpendicularForce, ForceMode.Force);
        
        if (forceInVeloDirection < 0f)
        {
            rb.AddForce(forceInVeloDirection * flatVel.normalized, ForceMode.Force);
        }
        
        LimitVelocity();
        NoInputDeceleration();
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
        // If our velocity is close to 0 and still not pressing an input, set velo to 0
        if (playerInput.moveVector.magnitude == 0f && flatVel.magnitude < 2f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
        // If player is not pressing any move button, decelerate them
        if (playerInput.moveVector.magnitude == 0f)
        {
            Debug.DrawRay(player.transform.position, -flatVel.normalized, Color.blue);
            rb.AddForce(-flatVel.normalized * stats.NoInputDeceleration);
        }

    }
}
