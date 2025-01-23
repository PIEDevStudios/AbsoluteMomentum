using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public float idleTime = 2f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        animator.Play("Idle");
    }
    public override void CheckTransitions()
    {
        base.CheckTransitions();

        if(stateUptime > idleTime)
        {
            isComplete = true;
        }

    }
    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }

}
