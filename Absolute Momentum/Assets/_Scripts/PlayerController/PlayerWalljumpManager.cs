using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;
using Unity.Netcode;
using UnityEngine;

public class PlayerWalljumpManager : NetworkBehaviour
{
    [SerializeField] private Player player;
    private float lastPressedJump;
    private WallSensor wallSensor => player.wallSensor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
    }

    // Update is called once per frame
    public void TickUpdate(PlayerInput.InputValues inputValues)
    {
        if (!IsOwner) return;
        
        if (player.stateMachine.currentState == player.wallrun)
        {
            if (inputValues.jumpPressedThisFrame || Time.time - lastPressedJump < player.stats.wallJumpBufferTime)
            {
                WallJump();
            }
        }
        else if (inputValues.jumpPressedThisFrame)
        {
            lastPressedJump = Time.time;
        }
    }

    private void WallJump()
    {
        Vector3 wallNormal = wallSensor.wallRight ? wallSensor.wallHitRight.normal : wallSensor.wallHitLeft.normal;
        
        // Cancel velocity in the direction of the wall
        Vector3 velocity = player.rb.linearVelocity;
        Vector3 velocityIntoWall = Vector3.Project(velocity, wallNormal);
        Vector3 correctedVelocity = velocity - velocityIntoWall;
        player.rb.linearVelocity = correctedVelocity;
        
        Vector3 forceToApply = transform.up * player.stats.wallJumpUpForce + wallNormal * player.stats.wallJumpSideForce;
        player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z);
        player.rb.AddForce(forceToApply, ForceMode.Impulse);
        player.wallrunResetTime = Time.time;
        player.stateMachine.SetState(player.airborne);
        
    }
    
}
