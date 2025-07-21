using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private float rayLength;
    [SerializeField] private CapsuleCollider playerCapsuleCollider;
    [SerializeField] private float radiusMultiplier = 1f;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float maxCoyoteTime = 0.1f;
    public bool IsGroundedCoyote => coyoteTimeCounter < maxCoyoteTime;
    public LayerMask groundLayer;
    public bool grounded { get; private set; }
    public RaycastHit hit { get; private set; }

    private void Update()
    {
        CheckGround();
        if (grounded)
            coyoteTimeCounter = 0;
        else
            coyoteTimeCounter += Time.deltaTime;
    }

    private void CheckGround()
    {
        // grounded = Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayer);
        grounded = Physics.SphereCast(transform.position, playerCapsuleCollider.radius * radiusMultiplier, Vector3.down, out RaycastHit _hit, rayLength, groundLayer);
        if (!grounded)
        {
            Debug.Log("NO GROUND");
        }
        hit = _hit;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position+Vector3.down*rayLength, playerCapsuleCollider.radius * radiusMultiplier);
    }
}
