using System;
using Unity.Netcode;
using UnityEngine;

public class DeathTrigger : NetworkBehaviour
{
    [SerializeField] private TriggerMode triggerMode;

    public enum TriggerMode
    {
        OnEnter,
        OnExit
    }

    private void HandlePlayerDeath(Collider other)
    {
        Player player = other.transform.root.GetComponentInChildren<Player>();
        player.RespawnPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Death Trigger entered by: {other.name}");
        // if (!IsServer) 
        // {
        //     Debug.Log("Death Trigger: Not server, returning");
        //     return;
        // }
        
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
        // if (!IsServer)
        // {
        //     Debug.Log("Death Trigger: Not server, returning");
        //     return;
        // }
        
        // Only handle death if we're in OnExit mode
        if (triggerMode == TriggerMode.OnExit)
        {
            Debug.Log("Death Trigger: OnExit mode, handling death");
            HandlePlayerDeath(other);
        }
    }
}
