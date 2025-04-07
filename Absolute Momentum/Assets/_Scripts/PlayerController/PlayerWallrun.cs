using UnityEngine;

public class PlayerWallrun : State
{
    [SerializeField] private Player player;
    private WallSensor wallSensor => player.wallSensor;    

    public override void DoEnterLogic()
    {
        
        rb.linearDamping = 0f;
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
        
        WallRunningMovement(inputValues);
    }


    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.ChangeGravity(player.stats.NormalGravity);
    }
    
    private void WallRunningMovement(PlayerInput.InputValues inputValues)
    {
        Vector3 wallNormal = wallSensor.wallRight ? wallSensor.wallHitRight.normal : wallSensor.wallHitLeft.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((player.orientation.forward - wallForward).magnitude > (player.orientation.forward + wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        
        player.playerObj.forward = wallForward;
        
        
        rb.AddForce(wallForward * player.stats.wallrunForce, ForceMode.Force);
        
        // Keep on wall force
        if (!(wallSensor.wallLeft && inputValues.moveVector.x > 0) && !(wallSensor.wallRight && inputValues.moveVector.x < 0))
        {
            rb.AddForce(-wallNormal * player.stats.stickToWallForce, ForceMode.Force);
        }
        
        
        
    }
}
