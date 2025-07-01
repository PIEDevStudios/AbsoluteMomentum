using UnityEngine;

public class PlayerWallSlide : State
{
    [SerializeField] private Player player;
    private WallSensor wallSensor => player.wallSensor;    

    public override void DoEnterLogic()
    {
        player.playerSpeedManager.currentCurve = player.stats.wallDragCurve;
        rb.linearDamping = 0f;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        if (rb.linearVelocity.y < 0f)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        base.DoEnterLogic();
        
    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }

    public override void DoTickUpdateState(PlayerInput.InputValues inputValues)
    {
        base.DoTickUpdateState(inputValues);
        if (!wallSensor.wallRight && !wallSensor.wallLeft && !wallSensor.wallForward)
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
        
        WallSlideMovement(inputValues);
        
    }


    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.wallrunResetTime = Time.time;
        player.ChangeGravity(player.stats.NormalGravity);
    }
    
    private void WallSlideMovement(PlayerInput.InputValues inputValues)
    {
        Vector3 wallNormal;

        if (wallSensor.wallRight)
        {
            wallNormal = wallSensor.wallHitRight.normal;
            player.playerObj.right = -wallNormal;
        }
        else if (wallSensor.wallLeft)
        {
            wallNormal = wallSensor.wallHitLeft.normal;
            player.playerObj.right = wallNormal;
        }
        else
        {
            wallNormal = wallSensor.wallHitForward.normal;
            player.playerObj.right = wallNormal;
        }
        
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        // Keep on wall force
        if (!(wallSensor.wallLeft && inputValues.moveVector.x > 0) && !(wallSensor.wallRight && inputValues.moveVector.x < 0))
        {
            rb.AddForce(-wallNormal * player.stats.stickToWallForce * flatVel.magnitude, ForceMode.Force);
        }
        
        
        
    }
}
