using UnityEngine;

public class WallJumpPad : BasePad
{
    [SerializeField] private float intensity;

    public override void ActivatePad(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        
        // Vector3 direction = gameObject.transform.right;
        // direction = direction.normalized;
        // direction *= rb.linearVelocity.magnitude * intensity;
        // //rb.linearVelocity = direction;
        // rb.AddForce(direction, ForceMode.VelocityChange);
        
        Vector3 force = gameObject.transform.right * intensity;
        rb.AddForce(force, ForceMode.Impulse);
    }
    
    
}
