using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NameTagGetter : NetworkBehaviour

[SerializeField] private Text nameTagText; // Assign this in the Inspector
private string playerName;

public override void OnNetworkSpawn()
{
    if (IsOwner)
    {
        // Set the player's username 
        playerName = "Player " + OwnerClientId;
        SetNameTagServerRpc(playerName);
    }

    // Listen for new players joining
    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
}

private void OnClientConnected(ulong clientId)
{
    if (IsServer)
    {
        // When a new player joins, update their name tag
        UpdateNameTag(clientId);
    }
}

private void UpdateNameTag(ulong clientId)
{
    // Get the player object for the connected client
    if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
    {
        var player = networkClient.PlayerObject.GetComponent<NameTagGetter>();
        if (player != null)
        {
            player.SetNameTagClientRpc(player.playerName);
        }
    }
}

[ServerRpc]
private void SetNameTagServerRpc(string name)
{
    playerName = name;
    SetNameTagClientRpc(name);
}

[ClientRpc]
private void SetNameTagClientRpc(string name)
{
    nameTagText.text = name;
}

public override void OnNetworkDespawn()
{
    // Clean up the event listener
    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
}