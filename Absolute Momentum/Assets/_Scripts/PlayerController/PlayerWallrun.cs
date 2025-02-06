using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerWallrun : State
{
    [SerializeField] private Player player;
    private WallSensor wallSensor => player.wallSensor;    

    public override void DoEnterLogic()
    {
        player.ChangeGravity(0);
        rb.linearDamping = 0f;
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
        
    }
}
