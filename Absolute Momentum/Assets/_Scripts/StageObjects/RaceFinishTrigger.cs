using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class RaceFinishTrigger : NetworkBehaviour
{
    [SerializeField] private string sceneToLoad;
    private HashSet<ulong> finishedPlayers = new HashSet<ulong>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.root.TryGetComponent(out NetworkObject networkObject))
        {
            // If this client is the owner of the object that collided, stop the timer on the client
            if (networkObject.IsOwner)
            {
                Player player = networkObject.GetComponentInChildren<Player>();
                player.playerRaceTimeManager.StopTimer();
                player.playerUI.DisplayResultsUI();
            }
                
            
            if (!IsServer) return; // Only the server should handle player finish tracking
            
            ulong playerId = networkObject.OwnerClientId;

            if (!finishedPlayers.Contains(playerId))
            {
                finishedPlayers.Add(playerId);
                Debug.Log($"Player {playerId} finished!");
                CheckAllPlayersFinished();
            }
        }
    }

    private void CheckAllPlayersFinished()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == finishedPlayers.Count)
        {
            Debug.Log("All players finished! Returning to lobby...");
            LoadLobbyClientRpc();
            TeleportAllPlayers(Vector3.up);
        }
    }

    [ClientRpc]
    private void LoadLobbyClientRpc()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }

    private void TeleportAllPlayers(Vector3 position)
    {
        foreach (var player in NetworkManager.Singleton.ConnectedClients)
        {
            player.Value.PlayerObject.GetComponentInChildren<PlayerPayloadManager>().TeleportPlayer(position);
        }
    }
}