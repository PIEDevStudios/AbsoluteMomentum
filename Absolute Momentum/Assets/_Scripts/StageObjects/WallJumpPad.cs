 using UnityEngine;

public class WallJumpPad : BasePad
{
    [SerializeField] private float intensity;
    [SerializeField] private Mode mode;

    public override void ActivatePad(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        
        // Vector3 direction = gameObject.transform.right;
        // direction = direction.normalized;
        // direction *= rb.linearVelocity.magnitude * intensity;
        // //rb.linearVelocity = direction;
        // rb.AddForce(direction, ForceMode.VelocityChange);
        if (mode == Mode.AdditiveImpulse)
        {
            Vector3 force = gameObject.transform.right * intensity;
            rb.AddForce(force, ForceMode.Impulse);
        }
        else if (mode == Mode.RedirectVelocity)
        {
            Vector3 newVelocity = Vector3.Reflect(rb.linearVelocity, gameObject.transform.right) * intensity;
            rb.AddForce(-rb.linearVelocity, ForceMode.VelocityChange);
            rb.AddForce(newVelocity, ForceMode.VelocityChange);
        }
        else if (mode == Mode.RedirectVelocityForced)
        {
            float speed = rb.linearVelocity.magnitude;
            Vector3 newVelocity = gameObject.transform.right * speed * intensity;
            rb.linearVelocity = newVelocity;
        }
    }
    
    public enum Mode {
        AdditiveImpulse,
        RedirectVelocity,
        RedirectVelocityForced
    }
    
}
