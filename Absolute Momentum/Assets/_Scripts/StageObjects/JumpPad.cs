using UnityEngine;

public class JumpPad : BasePad
{
    [SerializeField] private float jumpAmount;


    public override void ActivatePad(Rigidbody rb)
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
        rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);
    }
    
    
}
