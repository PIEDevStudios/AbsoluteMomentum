using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class that all state scriptable objects inherit from.
/// </summary>
public class State : MonoBehaviour
{
    protected StateMachineCore core;

    protected Rigidbody2D rb => core.rb;
    protected Animator animator => core.animator;
    public bool isComplete { get; protected set; }

    protected float stateUptime = 0;

    public StateMachine stateMachine;

    public StateMachine parent;
    public State currentState => stateMachine.currentState;
    public void SetState(State _newState, bool _forceReset = false)
    {
        stateMachine.SetState(_newState, _forceReset);
    }

    /// <summary>
    /// Passes the StateMachineCore to this state.
    /// Can be overriden to initialize additional parameters
    /// </summary>
    /// <param name="_core"></param>
    /// <param name="_parent"</param>
    public virtual void SetCore(StateMachineCore _core)
    {
        stateMachine = new StateMachine();
        core = _core;
    }
    /// <summary>
    /// Setup state, e.g. starting animations.
    /// Consider this the "Start" method of this state.
    /// </summary>
    public virtual void DoEnterLogic() { }

    /// <summary>
    /// State-Cleanup.
    /// </summary>
    public virtual void DoExitLogic() { currentState?.DoExitLogic(); ResetValues(); }

    /// <summary>
    /// This method is called once every frame while this state is active.
    /// Consider this the "Update" method of this state.
    /// </summary>
    public virtual void DoUpdateState() { CheckTransitions(); HandleTimer(); }

    /// <summary>
    /// This method is called once every physics frame while this state is active.
    /// Consider this the "FixedUpdate" method of this state.
    /// </summary>
    public virtual void DoFixedUpdateState() { }

    /// <summary>
    /// This method is called during ExitLogic().
    /// Use this method to reset or null out values during state cleanup.
    /// </summary>
    public virtual void ResetValues() { stateUptime = 0f; isComplete = false; }

    /// <summary>
    /// This method contains checks for all transitions from the current state.
    /// To be called in the UpdateState() or FixedUpdateState() methods.
    /// </summary>
    public virtual void CheckTransitions() { }


    protected void HandleTimer()
    {
        stateUptime += Time.deltaTime;
    }

    /// <summary>
    /// Calls DoUpdate for every state down the branch
    /// </summary>
    public void DoUpdateBranch()
    {
        currentState?.DoUpdateBranch();
        DoUpdateState();
    }

    /// <summary>
    /// Calls DoUpdate for every state down the branch
    /// </summary>
    public void DoFixedUpdateBranch()
    {
        currentState?.DoFixedUpdateBranch();
        DoFixedUpdateState();
    }

}