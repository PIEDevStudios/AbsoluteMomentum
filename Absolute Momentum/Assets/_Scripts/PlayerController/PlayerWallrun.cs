using UnityEditor.Experimental.GraphView;
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
        if (!wallSensor.wallRight && !wallSensor.wallLeft)
        {
            isComplete = true;
        }

        if (rb.linearVelocity.y < 0f && player.stats.CurrentGravity != player.stats.WallrunGravity)
        {
            player.ChangeGravity(player.stats.WallrunGravity);
        }
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        WallRunningMovement();
    }


    public override void DoExitLogic()
    {
        base.DoExitLogic();
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
        
        player.playerObj.forward = wallForward;
        
        
        rb.AddForce(wallForward * player.stats.wallrunForce, ForceMode.Force);
        
        // Keep on wall force
        if (!(wallSensor.wallLeft && player.playerInput.moveVector.x > 0) && !(wallSensor.wallRight && player.playerInput.moveVector.x < 0))
        {
            rb.AddForce(-wallNormal * 100f, ForceMode.Force);
        }
        
        
    }
}
