using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private float rayLength;
    [SerializeField] private CapsuleCollider playerCapsuleCollider;
    public LayerMask groundLayer;
    public bool grounded { get; private set; }

    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        // grounded = Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayer);
        grounded = Physics.SphereCast(new Ray(transform.position, Vector3.down), playerCapsuleCollider.radius, rayLength, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position+Vector3.down*rayLength, playerCapsuleCollider.radius);
    }
}
