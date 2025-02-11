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
                // Use player's movement direction for boosting
                Vector3 boostDirection = playerRigidbody.linearVelocity.magnitude > 0.1f
                    ? playerRigidbody.linearVelocity.normalized // Move in current direction
                    : playerInput.transform.forward; // Default to facing direction if not moving

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
