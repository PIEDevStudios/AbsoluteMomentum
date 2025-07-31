using Unity.Cinemachine;
using UnityEngine;

public class PlayerWallrun : State
{
    [SerializeField] private Player player;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform wallrunCam, defaultCam;
    [SerializeField] private CinemachineOrbitalFollow cinemachineCam;
    [SerializeField] private float cameraAlignSpeed = 5f;
    private WallSensor wallSensor => player.wallSensor;
    private Vector3 wallForward, wallNormal;

    public override void DoEnterLogic()
    {
        player.playerSpeedManager.currentCurve = player.stats.wallDragCurve;
        rb.linearDamping = 0f;
        CalculateWallNormal();
        // defaultCam.gameObject.SetActive(false);
        // wallrunCam.gameObject.SetActive(true);
        if (rb.linearVelocity.y < player.stats.minWallYSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, player.stats.minWallYSpeed, rb.linearVelocity.z);
        base.DoEnterLogic();
        
    }

    public override void DoUpdateState()
    {
        Debug.DrawRay(transform.position, wallForward * 10f, Color.yellow);
        AlignCamera(wallForward);
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
        // ResetCameraAlignment(player.playerObj.forward);
        defaultCam.gameObject.SetActive(true);
        wallrunCam.gameObject.SetActive(false);
        player.ChangeGravity(player.stats.NormalGravity);
                

    }
    
    private void WallRunningMovement()
    {
        CalculateWallNormal();


        
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

        if (!(wallSensor.wallLeft || wallSensor.wallRight))
        {
            Debug.Log("No Wall detected");
        }
        
        
    }

    private void AlignCamera(Vector3 wallForward)
    {
        // Smoothly rotate the camera target to match wallForward
        float targetAngle = Quaternion.LookRotation(wallForward).eulerAngles.y;
        cinemachineCam.HorizontalAxis.Value = Mathf.LerpAngle(cinemachineCam.HorizontalAxis.Value, targetAngle, cameraAlignSpeed * Time.fixedDeltaTime);
    }
    
    public void ResetCameraAlignment(Vector3 playerForward)
    {
        Quaternion resetRotation = Quaternion.LookRotation(playerForward, Vector3.up);
        cameraTarget.rotation = resetRotation;
    }

    private void CalculateWallNormal()
    {
        wallNormal = wallSensor.wallRight ? wallSensor.wallHitRight.normal : wallSensor.wallHitLeft.normal;
        wallForward = Vector3.Cross(wallNormal, Vector3.up);
        
        // Flip if the player is facing the opposite direction
        if ((player.playerObj.forward - wallForward).magnitude > (player.playerObj.forward + wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
    }
}
