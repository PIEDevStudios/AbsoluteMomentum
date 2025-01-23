using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : StateMachineCore
{
    public PatrolState patrolState;

    // Start is called before the first frame update
    private void Start()
    {
        SetupInstances();
        stateMachine.SetState(patrolState);
    }

    private void Update()
    {
        stateMachine?.currentState.DoUpdateBranch();
    }

    private void FixedUpdate()
    {
        stateMachine?.currentState.DoFixedUpdateBranch();
    }

}
