#define I2
#define AllowVelocityChangeMidDash
#define ReorientVelocity

using Unity.VisualScripting;
using UnityEngine;

public class PlayerDash : State
{
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject camera;
    private Vector3 lastVelocity;

    [DoNotSerialize] public float timeInDash;
    private PlayerStats stats => player.stats;
    private PlayerInput playerInput => player.playerInput;
    

    private Vector3 lastMovementVector;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        player.ChangeGravity(0);
        lastVelocity = player.rb.linearVelocity;
        timeInDash = 0;
        
#if I1
        lastMovementVector = Vector3.ProjectOnPlane(player.rb.linearVelocity, Vector3.up).normalized;
        if (lastMovementVector == Vector3.zero)
        {
            lastMovementVector = orientation.forward;
        }
        SetDashVelocity();
    }
#elif I2   
        lastMovementVector = player.rb.linearVelocity.normalized;
        if (lastMovementVector == Vector3.zero)
        {
            lastMovementVector = orientation.forward;
        }
        SetDashVelocity();
    }
    
#elif I3
        lastMovementVector = camera.transform.forward;

        SetDashVelocity();
    }

    
#endif
#if AllowVelocityChangeMidDash
    private void SetDashVelocity()
    {
#if I1
        Vector3 dashDir = (orientation.forward * playerInput.moveVector.y + orientation.right * playerInput.moveVector.x).normalized;
#elif I3
        Vector3 dashDir = (camera.transform.forward * playerInput.moveVector.y + camera.transform.right * playerInput.moveVector.x).normalized;
#endif
        if (dashDir == Vector3.zero)
        {
            dashDir = lastMovementVector;
        }
        
        dashDir = Vector3.Slerp(lastMovementVector, dashDir, 0.02f).normalized;
        player.rb.linearVelocity = dashDir * stats.DashSpeed;

        lastMovementVector = dashDir;
    }
#else
    private void SetDashVelocity()
    {
        player.rb.linearVelocity = lastMovementVector * stats.DashSpeed;
    }
#endif
    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.ChangeGravity(stats.NormalGravity);

    #if ReorientVelocity
        player.rb.linearVelocity = lastVelocity.magnitude * lastMovementVector;
    #else
        player.rb.linearVelocity = lastVelocity;
    #endif
    }
    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        timeInDash += Time.deltaTime;
        SetDashVelocity();
        
        if (timeInDash >= stats.DashTime)
        {
            isComplete = true;
        }
    }
    }