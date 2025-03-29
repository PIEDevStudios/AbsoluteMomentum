using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : NetworkSingletonPersistent<RaceManager>
{
    [SerializeField] private float countdownTime = 3f;
    [SerializeField] private Vector3 startPosition;

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
        playerReadyStatus[clientId] = false;
        
    }
    

    // Call this method from each player's script once their scene has loaded
    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoadedSceneServerRpc(ulong clientId)
    {
        if (playerReadyStatus.ContainsKey(clientId))
        {
            Debug.Log($"Player {clientId} loaded!");
            playerReadyStatus[clientId] = true;
            TeleportPlayerToStart(clientId);
            CheckAllPlayersReady();
        }
    }

    private void TeleportPlayerToStart(ulong clientId)
    {
        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponentInChildren<Player>();
        player.rb.position = startPosition;
    }
    
    private void CheckAllPlayersReady()
    {
        foreach (bool isReady in playerReadyStatus.Values)
        {
            if (!isReady)
                return; // Still waiting for others
        }

        // All players ready, start countdown
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
        // Implement race start logic here (e.g., enable player controls)
    }

    // Optional: Getter for countdown timer for clients to read
    public float GetCountdownTimer()
    {
        return countdownTimer.Value;
    }
}
