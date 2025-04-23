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

        var playerPayload = other.transform.root.GetComponentInChildren<PlayerPayloadManager>();
        if (playerPayload != null)
        {
            var player = playerPayload.GetComponent<Player>();
            if (player != null)
            {
                player.TriggerDeathScreen(0); // ✅ Call on the actual Player script
            }

            playerPayload.TeleportPlayer(respawnPosition);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        if (triggerMode != TriggerMode.OnExit) return;

        var playerPayload = other.transform.root.GetComponentInChildren<PlayerPayloadManager>();
        if (playerPayload != null)
        {
            playerPayload.TeleportPlayer(respawnPosition);
        }
    }
}
