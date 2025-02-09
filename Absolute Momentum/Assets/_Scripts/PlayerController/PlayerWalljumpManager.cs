using UnityEngine;

public class PlayerWalljumpManager : MonoBehaviour
{
    [SerializeField] private Player player;
    private WallSensor wallSensor => player.wallSensor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (player.stateMachine.currentState == player.wallrun)
        {
            if (player.playerInput.jumpPressedThisFrame)
            {
                WallJump();
            }
        }
    }

    private void WallJump()
    {
        Vector3 wallNormal = wallSensor.wallRight ? wallSensor.wallHitRight.normal : wallSensor.wallHitLeft.normal;
        
        Vector3 forceToApply = transform.up * player.stats.wallJumpUpForce + wallNormal * player.stats.wallJumpSideForce;
        
        player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z);
        player.rb.AddForce(forceToApply, ForceMode.Impulse);
        player.wallrunResetTimer = player.stats.wallrunResetTime;
        
    }
    
}
