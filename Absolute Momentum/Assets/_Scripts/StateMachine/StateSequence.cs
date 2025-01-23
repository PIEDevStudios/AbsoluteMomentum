using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This state plays all states in the list in order one after another
public class StateSequence : State
{
    [SerializeField] private List<State> states;
    private int currentStateIndex;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        currentStateIndex = 0;
        stateMachine.SetState(states[currentStateIndex], true);
    }
    public override void CheckTransitions()
    {
        base.CheckTransitions();
        //Debug.Log(gameObject.name + " Ind: " + currentStateIndex + "Count: " + states.Count);

        if (!currentState.isComplete) return;
        // If we are not on the last state go to the next state
        if (currentStateIndex != states.Count - 1)
        {
            currentStateIndex++;
            //Debug.Log(gameObject.name + " Changing State to " + states[currentStateIndex]);
            stateMachine.SetState(states[currentStateIndex], true);
        }
        // If we are on the last state, mark this state as true
        else
        {
            isComplete = true;
        }
    }
}
