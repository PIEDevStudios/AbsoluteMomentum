using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BoostTrigger : MonoBehaviour
{
    public float forceAmount = 25f;
    
    private PlayerInput cachedPlayerInput;
    private Rigidbody cachedRigidbody;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        // Cache the PlayerInput component on entry
        cachedPlayerInput = other.GetComponentInParent<PlayerInput>();
        
        if (cachedPlayerInput != null)
        {
            cachedRigidbody = cachedPlayerInput.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (cachedPlayerInput != null && cachedRigidbody != null)
        {
            Vector3 boostDirection;

            // Check if the player is moving (velocity magnitude > 0.1)
            if (cachedRigidbody.linearVelocity.magnitude > 0.1f)
            {
                // Use the player's movement direction for boosting
                boostDirection = cachedRigidbody.linearVelocity.normalized;
            }
            else
            {
                // Default to the direction the player is facing
                boostDirection = Vector3.zero;
            }

            // Apply force
            cachedRigidbody.AddForce(boostDirection * forceAmount, ForceMode.VelocityChange);
            
            Debug.Log("Boost applied in direction: " + boostDirection);
        }
        else
        {
            Debug.Log("Object inside trigger is not the player or missing components.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear cached references on exit
        if (cachedPlayerInput == other.GetComponentInParent<PlayerInput>())
        {
            cachedPlayerInput = null;
            cachedRigidbody = null;
        }
    }
}