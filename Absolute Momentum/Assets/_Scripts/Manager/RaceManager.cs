using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : NetworkSingletonPersistent<RaceManager>
{
    [SerializeField] private float countdownTime = 3f;
    [SerializeField] private Vector3[] startPositions;

    private int currentTeleportIndex;

    // Keep track of players
    private Dictionary<ulong, bool> playerReadyStatus = new Dictionary<ulong, bool>();

    // Network variable for countdown timer
    private NetworkVariable<float> countdownTimer = new NetworkVariable<float>(-1f, NetworkVariableReadPermission.Everyone);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += AddClientReadyStatus;
        }
    }

    private void AddClientReadyStatus(ulong clientId)
    {
        if (!IsServer) return;

        if (!playerReadyStatus.ContainsKey(clientId))
        {
            playerReadyStatus[clientId] = false;
        }
    }

    // Called by players when their scene is fully loaded and they are ready
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void MarkPlayerSceneReadyServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        
        if (!playerReadyStatus.ContainsKey(clientId))
            playerReadyStatus[clientId] = false;

        Debug.Log($"[Server] Player {clientId} marked ready at time: {NetworkManager.Singleton.ServerTime.Time:F2} seconds");

        playerReadyStatus[clientId] = true;
        TeleportPlayerToStart(clientId);
        CheckAllPlayersReady();
    }

    private void TeleportPlayerToStart(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            var player = client.PlayerObject.GetComponentInChildren<PlayerPayloadManager>();
            if (player != null)
            {
                player.TeleportPlayer(startPositions[currentTeleportIndex % startPositions.Length]);
                currentTeleportIndex++;
            }
        }
    }

    private void CheckAllPlayersReady()
    {
        foreach (var isReady in playerReadyStatus.Values)
        {
            if (!isReady)
            {
                Debug.Log($"Player is not Ready");
                return;
            }
        }

        Debug.Log("All players are ready. Starting race countdown.");
        StartCoroutine(BeginCountdown());
    }

    private IEnumerator BeginCountdown()
    {
        countdownTimer.Value = countdownTime;

        while (countdownTimer.Value > 0)
        {
            Debug.Log("Countdown: " + countdownTimer.Value);
            yield return new WaitForSeconds(1f);
            countdownTimer.Value--;
        }

        StartRace();
    }

    private void StartRace()
    {
        Debug.Log("Race Started!");
        // Additional race-start logic goes here (e.g., enabling movement)
    }

    public float GetCountdownTimer()
    {
        return countdownTimer.Value;
    }

    public void ResetRaceManagerValues()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            playerReadyStatus[clientId] = false;
        }

        currentTeleportIndex = 0;
        countdownTimer.Value = countdownTime;
    }
}
