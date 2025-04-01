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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (triggerMode != TriggerMode.OnEnter) return;
        if (other.transform.root.GetComponentInChildren<PlayerPayloadManager>() != null)
        {
            PlayerPayloadManager player = other.transform.root.GetComponentInChildren<PlayerPayloadManager>();
            player.TeleportPlayer(respawnPosition);
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        if (triggerMode != TriggerMode.OnExit) return;
        if (other.transform.root.GetComponentInChildren<PlayerPayloadManager>() != null)
        {
            PlayerPayloadManager player = other.transform.root.GetComponentInChildren<PlayerPayloadManager>();
            player.TeleportPlayer(respawnPosition);
        }

    }
}
