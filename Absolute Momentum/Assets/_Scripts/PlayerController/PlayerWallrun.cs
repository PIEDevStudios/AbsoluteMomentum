using UnityEngine;

public class PlayerWallrun : State
{
    [SerializeField] private Player player;
    private WallSensor wallSensor => player.wallSensor;    

    public override void DoEnterLogic()
    {
        player.playerSpeedManager.currentCurve = player.stats.wallDragCurve;
        rb.linearDamping = 0f;
        if (rb.linearVelocity.y < player.stats.minWallYSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, player.stats.minWallYSpeed, rb.linearVelocity.z);
        base.DoEnterLogic();
        
    }

    public override void DoUpdateState()
    {
        if (player.playerInput.moveVector.y != 0) return;
        if (wallSensor.wallRight && player.playerInput.moveVector.x < 0f || wallSensor.wallLeft && player.playerInput.moveVector.x > 0f)
        {
            isComplete = true;
        }
        
        base.DoUpdateState();
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        if (!wallSensor.wallRight && !wallSensor.wallLeft)
        {
            Debug.Log("No Wall detected");
            isComplete = true;
        }

        if (player.groundSensor.grounded)
        {
            Debug.Log("Player Hit Ground");
            isComplete = true;
        }

        if (rb.linearVelocity.y < 0f && player.stats.CurrentGravity != player.stats.WallrunGravity)
        {
            player.ChangeGravity(player.stats.WallrunGravity);
        }
        
        WallRunningMovement();
        
        if (wallSensor.wallForward)
        {
            Debug.Log("Wall Forward");
            isComplete = true;
        }
    }


    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.wallrunResetTime = Time.time;
        player.ChangeGravity(player.stats.NormalGravity);
    }
    
    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallSensor.wallRight ? wallSensor.wallHitRight.normal : wallSensor.wallHitLeft.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((player.orientation.forward - wallForward).magnitude > (player.orientation.forward + wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        
        player.playerObj.forward = Vector3.Slerp(player.playerObj.forward, wallForward, player.stats.wallrunTurnSpeed * Time.deltaTime);
        
        // Vector3 currentForward = player.playerObj.forward;
        // player.playerObj.forward = Vector3.Slerp(currentForward, wallForward, player.stats.wallrunTurnSpeed);

        rb.AddForce(wallForward * player.stats.wallrunForce, ForceMode.Force);
        
        // rb.AddForce(wallForward * player.stats.wallrunForce, ForceMode.Force);
        
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        // Keep on wall force
        if (!(wallSensor.wallLeft && player.playerInput.moveVector.x > 0) && !(wallSensor.wallRight && player.playerInput.moveVector.x < 0))
        {
            rb.AddForce(-wallNormal * player.stats.stickToWallForce * flatVel.magnitude, ForceMode.Force);
        }
        
        
        
    }
}
