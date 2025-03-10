using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static UnityEngine.KeyCode;

public class PlayerInput : NetworkBehaviour
{
    [SerializeField] private InputActionAsset playerInputAction;
    public Vector2 moveVector { get; private set; }
    public Vector2 lookVector { get; private set; }
    public float timeOfLastMoveInput { get; private set; }
    public bool jumpPressedThisFrame { get; private set; }
    public bool jumpReleasedThisFrame { get; private set; }
    public bool jumpHeld { get; private set; }
    public bool sprintHeld { get; private set; }
    public bool sprintPressedThisFrame { get; private set; }
    
    public bool slidePressedThisFrame { get; private set; }
    public bool slideReleasedThisFrame { get; private set; }
    public bool slideHeld { get; private set; }
    
    public bool ResetInput { get; private set;}

    public struct InputValues
    {
        public Vector2 moveVector;
        public Vector2 lookVector;
        public float timeOfLastMoveInput;
        public bool jumpPressedThisFrame;
        public bool jumpReleasedThisFrame;
        public bool jumpHeld;
        public bool sprintHeld;
        public bool sprintPressedThisFrame;
        public bool slidePressedThisFrame;
        public bool slideReleasedThisFrame;
        public bool slideHeld;
        public bool ResetInput;
    }
    
    
    private void OnEnable()
    {
        if (!IsOwner) return;
        playerInputAction.Enable();
    }

    private void OnDisable()
    {
        playerInputAction.Disable();
    }
    
    public void TickUpdate()
    {
        InputActionMap playerActionMap = playerInputAction.actionMaps[0];
        moveVector = playerActionMap.FindAction("Move").ReadValue<Vector2>();
        
        
        if (moveVector.magnitude > 0)
        {
            timeOfLastMoveInput = Time.time;
        }
        
        
        // Jump
        bool heldLastFrame = jumpPressedThisFrame;
        jumpHeld = playerActionMap.FindAction("Jump").ReadValue<float>() > 0;
        jumpPressedThisFrame = !heldLastFrame && jumpHeld;
        jumpReleasedThisFrame = heldLastFrame && !jumpHeld;
        
        // Sprint
        heldLastFrame = sprintHeld;
        sprintHeld = playerActionMap.FindAction("Sprint").ReadValue<float>() > 0;
        sprintPressedThisFrame = !heldLastFrame && sprintHeld;
        
        heldLastFrame = slideHeld;
        slideHeld = playerActionMap.FindAction("Slide").ReadValue<float>() > 0;
        slidePressedThisFrame = !heldLastFrame && slideHeld;
        slideReleasedThisFrame = heldLastFrame && !slideHeld;
        
        
        // DEBUG INPUTS
        ResetInput = Input.GetKeyDown(R);
    }

    public InputValues GetInputValues()
    {
        return new InputValues()
        {
            moveVector = moveVector,
            lookVector = lookVector,
            timeOfLastMoveInput = timeOfLastMoveInput,
            jumpPressedThisFrame = jumpPressedThisFrame,
            jumpReleasedThisFrame = jumpReleasedThisFrame,
            jumpHeld = jumpHeld,
            sprintPressedThisFrame = sprintPressedThisFrame,
            sprintHeld = sprintHeld,
            slidePressedThisFrame = slidePressedThisFrame,
            slideReleasedThisFrame = slideReleasedThisFrame,
            slideHeld = slideHeld,
            ResetInput = ResetInput
        };
    }
        
        
}
