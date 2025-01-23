using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public IdleState idleState;
    public NavigateState navigateState;
    public Transform anchor1, anchor2;

    private void GoToNextDestination()
    {
        float randomSpot = Random.Range(anchor1.position.x, anchor2.position.x);
        navigateState.destination = new Vector2(randomSpot, core.transform.position.y);
        stateMachine.SetState(navigateState);
    }


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        GoToNextDestination();
        stateMachine.SetState(idleState);
    }

    public override void CheckTransitions()
    {
        base.CheckTransitions();
        if (!currentState.isComplete) return;
        if(currentState == idleState)
        {
            GoToNextDestination();
        }
        else
        {
            stateMachine.SetState(idleState);
        }
    }


}
