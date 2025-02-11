using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    public float forceAmount = 25f; 

    private void OnTriggerStay(Collider other)
    {
        // Get the root Player object using PlayerInput
        PlayerInput playerInput = other.GetComponentInParent<PlayerInput>();

        if (playerInput != null)
        {
            // Get Rigidbody from the Player
            Rigidbody playerRigidbody = playerInput.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                Vector3 boostDirection;

                // Check if the player is moving (velocity magnitude > 0.1)
                if (playerRigidbody.linearVelocity.magnitude > 0.1f)
                {
                    // Use the player's movement direction for boosting
                    boostDirection = playerRigidbody.linearVelocity.normalized;
                }
                else
                {
                    // Default to the direction the player is facing
                    boostDirection = playerInput.transform.forward;
                }

                // Apply force
                playerRigidbody.AddForce(boostDirection * forceAmount, ForceMode.VelocityChange);
                
                Debug.Log("Boost applied in direction: " + boostDirection);
            }
            else
            {
                Debug.LogWarning("Player has no Rigidbody.");
            }
        }
        else
        {
            Debug.Log("Object inside trigger is not the player.");
        }
    }
}
