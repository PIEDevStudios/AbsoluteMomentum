using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class StateMachineCore : NetworkBehaviour
{
    /// <summary>
    /// Dictonary used to hold states that are NOT a part of a heirarchical state machine.
    /// </summary>
    [Header("StateMachineCore Variables")]
    public Rigidbody rb;
    /// <summary>
    /// GameObject that is the parent to all states in this stateMachineCore
    /// </summary>
    public GameObject statesParent;
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

        State[] _allChildStates;

        if (statesParent == null)
        {
            _allChildStates = GetComponentsInChildren<State>();
        }
        else
        {
            _allChildStates = statesParent.GetComponentsInChildren<State>();
        }

        foreach (State _state in _allChildStates)
        {
            _state.SetCore(this);
            _state.gameObject.SetActive(false);
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
        if (Application.isPlaying && stateMachine != null)
        {
            List<State> states = stateMachine.GetActiveStateBranch();
    
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            if(stateMachine.currentState.isComplete)
            {
                style.normal.textColor = Color.green;
            }
            else
            {
                style.normal.textColor = Color.red;
            }
            style.fontSize = 40;
            UnityEditor.Handles.Label(transform.position + Vector3.up, "Active States: " + string.Join(" > ", states), style);
        
        }
        #endif
    }


   
}
