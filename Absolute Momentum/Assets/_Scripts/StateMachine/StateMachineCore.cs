using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineCore : MonoBehaviour
{
    /// <summary>
    /// Dictonary used to hold states that are NOT a part of a heirarchical state machine.
    /// </summary>
    public Rigidbody rb;
    public Animator animator;
    public StateMachine stateMachine { get; private set; }
    /// <summary>
    /// Passes the core to all the states in the states dictionary
    /// </summary>
    /// 
    private void Start()
    {
        // Template for classes that inherit from this class
        // put the following code inside of your Start() function

        //SetupInstances()
        //stateMachine.SetState(patrolState);
    }
    public void SetupInstances()
    {
        stateMachine = new StateMachine();

        State[] _allChildStates = GetComponentsInChildren<State>();

        foreach (State _state in _allChildStates)
        {
            _state.SetCore(this);
        }
    }

    private void Update()
    {
        // Template for classes that inherit from this class
        // put the following code inside of your Update() function

        //if(stateMachine.currentState.isComplete)
        //{
            //Default state transition if the state completes itself
        //}
        //stateMachine.currentState.DoUpdateBranch();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.DoFixedUpdateBranch();
    }

    public virtual void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            List<State> states = stateMachine.GetActiveStateBranch();

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3, "Active States: " + string.Join(" > ", states), style);
        
        }
        #endif
    }


   
}
