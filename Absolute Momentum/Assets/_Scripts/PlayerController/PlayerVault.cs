using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerVault : State
{
    [SerializeField] private Player player;
    private Vector3 ledgePoint;
    [SerializeField] private float climbDuration = 0.4f, climbUpOffset = 1.8f, wallOffset = 0.3f;
    private Vector3 climbTargetPos;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player.ChangeGravity(0f);
        rb.linearVelocity = Vector3.zero;
        
        // Cache grab position
        player.VaultSensor.CheckForLedge(out RaycastHit ledge);
        ledgePoint = ledge.point;
        Vector3 ledgeNormal = ledge.normal;

        // Calculate the final climb position
        climbTargetPos = ledgePoint 
                                 - (ledgeNormal * wallOffset)   // move back from the wall
                                 + (Vector3.up * climbUpOffset); // move up to the top

        
        ledgePoint.y -= 0.3f; // adjust so hands line up visually
        rb.position = Vector3.Lerp(transform.position, ledgePoint, 0.5f);
        StartCoroutine(ClimbUp(climbTargetPos));
    }
    
    IEnumerator ClimbUp(Vector3 targetPosition)
    {
        Debug.Log("Started Climbing Up");
        float elapsed = 0f;
        Vector3 start = rb.position;

        while (elapsed < climbDuration)
        {
            rb.MovePosition(Vector3.Lerp(start, targetPosition, elapsed / climbDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.position = targetPosition;
        isComplete = true;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        player.ChangeGravity(player.stats.NormalGravity);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        ledgePoint = Vector3.zero;
        climbTargetPos = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(climbTargetPos, 0.2f);
    }
}
