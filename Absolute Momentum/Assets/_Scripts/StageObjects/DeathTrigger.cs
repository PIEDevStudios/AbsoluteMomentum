using System;
using Unity.Netcode;
using UnityEngine;

public class DeathTrigger : NetworkBehaviour
{
    [SerializeField] private TriggerMode triggerMode;
    [SerializeField] private Vector3 respawnPosition;

    public enum TriggerMode
    {
        OnEnter,
        OnExit
    }

    private void HandlePlayerDeath(Collider other)
    {
        Debug.Log("PLAYER DIES");
        var playerPayload = other.transform.root.GetComponentInChildren<PlayerPayloadManager>();
        if (playerPayload != null)
        {
            Debug.Log("Death Trigger: Found player payload");
            var player = playerPayload.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Death Trigger: Triggering death screen");
                player.TriggerDeathScreen(4);
            }
            else
            {
                Debug.Log("Death Trigger: Player component not found");
            }

            playerPayload.TeleportPlayer(respawnPosition);
        }
        else
        {
            Debug.Log("Death Trigger: PlayerPayload not found");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Death Trigger entered by: {other.name}");
        if (!IsServer) 
        {
            Debug.Log("Death Trigger: Not server, returning");
            return;
        }
        
        // Only handle death if we're in OnEnter mode
        if (triggerMode == TriggerMode.OnEnter)
        {
            Debug.Log("Death Trigger: OnEnter mode, handling death");
            HandlePlayerDeath(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Death Trigger exited by: {other.name}");
        if (!IsServer)
        {
            Debug.Log("Death Trigger: Not server, returning");
            return;
        }
        
        // Only handle death if we're in OnExit mode
        if (triggerMode == TriggerMode.OnExit)
        {
            Debug.Log("Death Trigger: OnExit mode, handling death");
            HandlePlayerDeath(other);
        }
    }
}
