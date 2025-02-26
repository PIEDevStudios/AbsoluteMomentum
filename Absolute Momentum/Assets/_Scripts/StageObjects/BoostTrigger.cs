using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BoostTrigger : MonoBehaviour
{
    public float acceleration = 35f;
    
    private PlayerInput cachedPlayerInput;
    private Rigidbody cachedRigidbody;
    private Transform playerOrientation; // Reference to the player's orientation

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        cachedPlayerInput = other.GetComponentInParent<PlayerInput>();
        
        if (cachedPlayerInput != null)
        {
            cachedRigidbody = cachedPlayerInput.GetComponent<Rigidbody>();
            // Get the player's orientation component
            playerOrientation = cachedPlayerInput.transform.Find("Orientation");
            
            if (playerOrientation == null)
            {
                Debug.LogWarning("Player Orientation not found - using player's transform");
                playerOrientation = cachedPlayerInput.transform;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (cachedPlayerInput != null && cachedRigidbody != null && playerOrientation != null)
        {
            // Only apply boost if player is actually providing input
            if (cachedPlayerInput.moveVector.magnitude > 0.1f)
            {
                // Use player's orientation for movement direction
                Vector3 moveDirection = (playerOrientation.forward * cachedPlayerInput.moveVector.y + 
                                       playerOrientation.right * cachedPlayerInput.moveVector.x).normalized;
                
                // Check if we're moving roughly in the same direction as current velocity
                if (cachedRigidbody.linearVelocity.magnitude > 0.1f)
                {
                    float dotProduct = Vector3.Dot(moveDirection, cachedRigidbody.linearVelocity.normalized);
                    // If we're moving in a very different direction, don't apply boost
                    if (dotProduct < -0.5f)
                    {
                        return;
                    }
                }
                
                // Apply boost in the movement direction
                cachedRigidbody.AddForce(moveDirection * acceleration, ForceMode.VelocityChange);
                
                Debug.Log($"Boost applied in direction: {moveDirection}, Current Velocity: {cachedRigidbody.linearVelocity}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cachedPlayerInput == other.GetComponentInParent<PlayerInput>())
        {
            cachedPlayerInput = null;
            cachedRigidbody = null;
            playerOrientation = null;
        }
    }
}