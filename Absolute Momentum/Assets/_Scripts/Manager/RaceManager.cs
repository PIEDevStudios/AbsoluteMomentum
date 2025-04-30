using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RaceManager : NetworkSingletonPersistent<RaceManager>
{
    [SerializeField] private float countdownTime = 3f, introJingleDelay = 2f;
    [SerializeField] private Vector3[] startPositions;
    [field:SerializeField] public String[] levelNames { get; private set; }
    private int currentTeleportIndex;

    // Keep track of players
    private Dictionary<ulong, bool> playerReadyStatus = new Dictionary<ulong, bool>();
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    private Dictionary<ulong, int> playerCheckpoints = new Dictionary<ulong, int>();
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    [SerializeField] private int numCheckpoints;
    // Network variable for countdown timer
    private NetworkVariable<float> countdownTimer = new NetworkVariable<float>(-1f, NetworkVariableReadPermission.Everyone);

    public Action OnTimeSubmitted;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += AddClientReadyStatus;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (levelNames.Contains(scene.name))
        {
            playerRaceTimes.Clear();
        }
    }

    private void AddClientReadyStatus(ulong clientId)
    {
        if (!IsServer) return;

        if (!playerReadyStatus.ContainsKey(clientId))
        {
            playerReadyStatus[clientId] = false;
            playerCheckpoints[clientId] = 0;
            playerLaps[clientId] = 0;
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SubmitRaceTimeServerRPC(float time, RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        playerRaceTimes[clientId] = time;
        SubmitRaceTimeClientRPC(clientId, time);
    }
    
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void SubmitRaceTimeClientRPC(ulong clientId, float time)
    {
        playerRaceTimes[clientId] = time;
        OnTimeSubmitted?.Invoke();
    }
    
    // Called by players when their scene is fully loaded and they are ready
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void MarkPlayerSceneReadyServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        
        Debug.Log($"[Server] Player {clientId} marked ready at time: {NetworkManager.Singleton.ServerTime.Time:F2} seconds");

        playerReadyStatus[clientId] = true;
        CheckAllPlayersReady();
    }
    
    // Called by players when their scene is fully loaded and they are ready
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TeleportToStartServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
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
        countdownTimer.Value = -1;
        
        foreach (var isReady in playerReadyStatus.Values)
        {
            if (!isReady)
            {
                Debug.Log($"Player is not Ready");
                return;
            }
        }

        Debug.Log("All players are ready. Starting race countdown.");
        StartCountdownClientRPC();
        StartCoroutine(BeginCountdown());
    }

    private IEnumerator BeginCountdown()
    {
        yield return new WaitForSeconds(introJingleDelay);
        
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
        playerRaceTimes.Clear();
        StartRaceClientRPC();
        // Additional race-start logic goes here (e.g., enabling movement)
    }

    [ClientRpc]
    private void StartRaceClientRPC()
    {
        IntroAudioEvents.Instance.PlayAudio(0);
    }
    
    [ClientRpc]
    private void StartCountdownClientRPC()
    {
        IntroAudioEvents.Instance.PlayAudio(1);
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
    
    public Dictionary<ulong, float> getRaceTimes()
    {
        foreach (var player in playerRaceTimes)
        {
            Debug.Log("Player " + player.Key + " finished with a time of " + player.Value);
        }
        return playerRaceTimes;
    }

    public void UpdateCheckpoint(ulong playerID, int checkpointID)
    {
        int lastCheckpoint = playerCheckpoints[playerID];
        Debug.Log($"Trying to add checkpoint {checkpointID} to player {playerID} (CURRENT: {lastCheckpoint})");
        if (lastCheckpoint == checkpointID - 1) {
            if (checkpointID == numCheckpoints) {
                playerLaps[playerID] += 1;
                Debug.Log($"Player {playerID} Completed A Lap! (Current Lap: {playerLaps[playerID]}");
                playerCheckpoints[playerID] = 0;
            } else {
                playerCheckpoints[playerID] = checkpointID;
            }
        }
    }

    public int GetPlayerLaps(ulong playerID) {
        return playerLaps[playerID];
    }
}
