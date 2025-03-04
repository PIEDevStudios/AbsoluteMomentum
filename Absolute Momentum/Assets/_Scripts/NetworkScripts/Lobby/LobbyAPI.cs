using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;

    public static LobbyManager Instance { get; private set; }
    
    private Lobby currentLobby;
    private string playerName;

    // Time variables for periodic lobby updates
    private float heartbeatTimeMax = 15f;
    private float lobbyUpdateTimeMax = 1.1f;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Initialize UGS services as soon as possible
            // This tells the compiler you’re intentionally not awaiting the task.
            _ = InitializeServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize Unity Gaming Services and sign in
    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Generate a random player name and update the UI if set
            playerName = "Player" + UnityEngine.Random.Range(100, 999);
            if (usernameText != null)
            {
                usernameText.text = playerName;
            }

            // Initialize timers
            heartbeatTimer = heartbeatTimeMax;
            lobbyUpdateTimer = lobbyUpdateTimeMax;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to initialize Unity Services: " + e);
        }
    }

    private void Update()
    {
        // Periodically send heartbeat pings and update lobby info
        HandleLobbyHeartbeat();
        HandleLobbyPollUpdate();
    }

    // Sends a heartbeat to keep the lobby alive (for hosts)
    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby != null && IsHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = heartbeatTimeMax;
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                    Debug.Log("Heartbeat sent for lobby " + currentLobby.Id);
                }
                catch (Exception e)
                {
                    Debug.LogError("Heartbeat error: " + e);
                }
            }
        }
    }

    // Updates lobby information periodically (for joined lobbies)
    private async void HandleLobbyPollUpdate()
    {
        if (currentLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = lobbyUpdateTimeMax;
                try
                {
                    currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                    Debug.Log("Lobby info updated. Player count: " + currentLobby.Players.Count);
                }
                catch (Exception e)
                {
                    Debug.LogError("Lobby poll update error: " + e);
                }
            }
        }
    }

    // Creates a lobby and sets up a relay allocation for hosting
    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, string mapName, string gameMode)
    {
        try
        {
            // Create relay allocation for host (maxPlayers-1 because host is already connected)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            
            // Set relay data for the host transport component
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayData);
            
            // Start the host using Netcode for GameObjects
            NetworkManager.Singleton.StartHost();

            // Prepare lobby creation options with additional data
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = GetPlayerData(),
                Data = new Dictionary<string, DataObject>
                {
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public, mapName, DataObject.IndexOptions.S1) },
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode, DataObject.IndexOptions.S2) },
                    { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode, DataObject.IndexOptions.S3) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("Lobby created: " + currentLobby.Name + " (" + currentLobby.Id + ")");
        }
        catch (Exception e)
        {
            Debug.LogError("Error creating lobby: " + e);
        }
    }

    // Joins a lobby using a provided lobby code and connects to the relay
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayerData()
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            Debug.Log("Joined lobby: " + currentLobby.Name);

            // Retrieve the relay join code from lobby data
            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;
            
            // Join the relay allocation as a client
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            
            // Set relay data on the client transport component
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayData);
            
            // Start the client
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error joining lobby: " + e);
        }
    }

    // Creates a Player object with a name for UGS services
    private Unity.Services.Lobbies.Models.Player GetPlayerData()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    // Helper method to check if the current player is the lobby host
    private bool IsHost()
    {
        return currentLobby != null && AuthenticationService.Instance.PlayerId == currentLobby.HostId;
    }

    // Example method to leave the lobby (for both host and client)
    public async void LeaveLobby()
    {
        try
        {
            if (currentLobby != null)
            {
                // If host, delete the lobby; if client, simply remove yourself
                if (IsHost())
                {
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                }
                currentLobby = null;
                Debug.Log("Left the lobby.");
            }
            else
            {
                Debug.LogWarning("No lobby exists to leave.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error leaving lobby: " + e);
        }
    }

    // You can add additional methods (QuickJoin, KickPlayer, UpdateLobby, etc.) similarly.
}
