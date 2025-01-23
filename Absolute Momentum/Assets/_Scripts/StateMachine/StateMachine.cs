using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Allows for the creation of a state machine to define more advanced player, npc, and enemy behavior
/// </summary>
public class StateMachine
{
    public State currentState  { get; private set; } 
    public State previousState { get; private set; }

    /// <summary>
    /// Sets the state machine with a specified state
    /// </summary>
    /// <param name="_newState"></param>
    /// <param name="_forceReset"</param>
    public void SetState(State _newState, bool _forceReset = false)
    {
        if(currentState != _newState || _forceReset)
        {
            currentState?.DoExitLogic();
            previousState = currentState;
            currentState = _newState;
            currentState.DoEnterLogic();
        }

    }


    public List<State> GetActiveStateBranch(List<State> _list = null)
    {
        if (_list == null)
            _list = new List<State>();

        if (currentState == null)
            return _list;

        else
        {
            _list.Add(currentState);
            return currentState.stateMachine.GetActiveStateBranch(_list);
        }

    }

}