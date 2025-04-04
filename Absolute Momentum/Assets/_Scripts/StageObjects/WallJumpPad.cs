 using UnityEngine;

public class WallJumpPad : BasePad
{
    [SerializeField] private float intensity;
    [SerializeField] private Mode mode;
    [SerializeField] private NormalFace normalFace;
    private Vector3 direction;
    public enum NormalFace
    {
        Right,
        Up,
        Forward
    }

    private void Awake()
    {
        if (normalFace == NormalFace.Right)
        {
            direction = transform.right;
        }
        else if (normalFace == NormalFace.Up)
        {
            direction = transform.up;
        }
        else
        {
            direction = transform.forward;
        }
    }
    public override void ActivatePad(Rigidbody rb)
    {
        // Vector3 direction = gameObject.transform.right;
        // direction = direction.normalized;
        // direction *= rb.linearVelocity.magnitude * intensity;
        // //rb.linearVelocity = direction;
        // rb.AddForce(direction, ForceMode.VelocityChange);
        if (mode == Mode.AdditiveImpulse)
        {
            Vector3 force = direction * intensity;
            rb.AddForce(force, ForceMode.Impulse);
        }
        else if (mode == Mode.RedirectVelocity)
        {
            Vector3 newVelocity = Vector3.Reflect(rb.linearVelocity, direction) * intensity;
            rb.AddForce(-rb.linearVelocity, ForceMode.VelocityChange);
            rb.AddForce(newVelocity, ForceMode.VelocityChange);
        }
        else if (mode == Mode.RedirectVelocityForced)
        {
            float speed = rb.linearVelocity.magnitude;
            Vector3 newVelocity = direction * speed * intensity;
            rb.linearVelocity = newVelocity;
        }
    }
    
    public enum Mode {
        AdditiveImpulse,
        RedirectVelocity,
        RedirectVelocityForced
    }
    
}
