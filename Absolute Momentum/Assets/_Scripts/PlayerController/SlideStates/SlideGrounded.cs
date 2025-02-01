using UnityEngine;

public class SlideGrounded : State
{
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        
        // Add force down to stick the player to the ground
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
}
