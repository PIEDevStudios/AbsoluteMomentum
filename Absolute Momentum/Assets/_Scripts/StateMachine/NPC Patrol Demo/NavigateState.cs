using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigateState : State
{
    public Vector2 destination;

    public float speed = 1;
    public float threshold = 0.1f;

    // Generic animation state?
    // public State animation

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        animator.Play("Run");
    }


    public override void DoUpdateState()
    {
        base.DoUpdateState();
        if(Vector2.Distance(core.transform.position, destination) < threshold)
        {
            isComplete = true;
        }
        core.transform.localScale = new Vector3(Mathf.Sign(rb.linearVelocity.x) * 2f, core.transform.localScale.y, 1);
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        Vector2 direction = (destination - (Vector2) core.transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }
}
