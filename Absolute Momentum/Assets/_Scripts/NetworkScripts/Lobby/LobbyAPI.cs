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

public class LobbyAPI : SingletonPersistent<LobbyAPI>
{
    public event Action<List<Lobby>> LobbiesUpdated;

    [SerializeField] private TextMeshProUGUI usernameText;

    public static LobbyAPI Instance { get; private set; }
    
    private Lobby currentLobby;
    private string playerName;

    // Timer variables for heartbeat and lobby polling
    private float heartbeatTimeMax = 15f;
    private float lobbyUpdateTimeMax = 1.1f;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    public delegate void LobbyJoinedHandler(Lobby lobby);
    public event LobbyJoinedHandler OnLobbyJoined;

    private void Awake()
    {
        // Implement a singleton pattern.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Call async initialize without awaiting by discarding the returned Task.
            _ = InitializeServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes Unity Gaming Services and signs in the user.
    /// </summary>
    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Generate a random player name and update the UI if available.
            playerName = "Player" + UnityEngine.Random.Range(100, 999);
            if (usernameText != null)
            {
                usernameText.text = playerName;
            }

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
        HandleLobbyHeartbeat();
        HandleLobbyPollUpdate();
    }

    /// <summary>
    /// Sends periodic heartbeat pings to keep the lobby active (host only).
    /// </summary>
    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby != null && IsHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
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

    /// <summary>
    /// Polls the lobby for updates periodically.
    /// </summary>
    private async void HandleLobbyPollUpdate()
    {
        if (currentLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0f)
            {
                lobbyUpdateTimer = lobbyUpdateTimeMax;
                try
                {
                    currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                    Debug.Log("Lobby updated. Players count: " + currentLobby.Players.Count);
                }
                catch (Exception e)
                {
                    Debug.LogError("Lobby poll update error: " + e);
                }
            }
        }
    }

    /// <summary>
    /// Creates a lobby, sets up Relay for hosting, and starts the host.
    /// </summary>
    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, string mapName, string gameMode)
    {
        try
        {
            // Create a Relay allocation (subtracting one for the host).
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            // Configure UnityTransport with the Relay data.
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayData);

            // Start hosting using Netcode.
            NetworkManager.Singleton.StartHost();

            // Prepare lobby options including extra data.
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

    /// <summary>
    /// Joins a lobby using its code and sets up the Relay connection for a client.
    /// </summary>
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
            OnLobbyJoined?.Invoke(currentLobby);

            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error joining lobby by code: " + e);
        }
    }

    /// <summary>
    /// Joins a lobby by its ID and sets up the Relay connection.
    /// </summary>
    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayerData()
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
            Debug.Log("Joined lobby by ID: " + currentLobby.Name);

            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error joining lobby by ID: " + e);
        }
    }

    /// <summary>
    /// Quick joins an available lobby.
    /// </summary>
    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = GetPlayerData()
            };

            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            Debug.Log("Quick joined lobby: " + currentLobby.Name);

            string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error quick joining lobby: " + e);
        }
    }

    /// <summary>
    /// Lists available lobbies and logs key details.
    /// </summary>
    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            Debug.Log("Found " + response.Results.Count + " lobbies.");
            foreach (Lobby lobby in response.Results)
            {
                Debug.Log($"Lobby: {lobby.Name} | Players: {lobby.Players.Count}/{lobby.MaxPlayers} | Map: {lobby.Data["Map"].Value}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error listing lobbies: " + e);
        }
    }

    /// <summary>
    /// Kicks a player from the current lobby.
    /// </summary>
    public async void KickPlayer(string playerId)
    {
        try
        {
            if (currentLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
                Debug.Log("Kicked player: " + playerId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error kicking player: " + e);
        }
    }

    /// <summary>
    /// Migrates the lobby host to a new player if available.
    /// </summary>
    public async void MigrateLobbyHost()
    {
        try
        {
            if (currentLobby != null && currentLobby.Players.Count > 1)
            {
                string newHostId = currentLobby.Players[1].Id;
                currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
                {
                    HostId = newHostId
                });
                Debug.Log("Migrated lobby host to: " + newHostId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error migrating lobby host: " + e);
        }
    }

    /// <summary>
    /// Updates the lobby's map data (host only).
    /// </summary>
    public async void UpdateLobbyMap(string mapName)
    {
        try
        {
            if (currentLobby != null && IsHost())
            {
                currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "Map", new DataObject(DataObject.VisibilityOptions.Public, mapName, DataObject.IndexOptions.S1) }
                    }
                });
                Debug.Log("Updated lobby map to: " + mapName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating lobby map: " + e);
        }
    }

    /// <summary>
    /// Updates the player's name within the lobby.
    /// </summary>
    public async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            if (currentLobby != null)
            {
                await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                });
                Debug.Log("Updated player name to: " + playerName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating player name: " + e);
        }
    }

    /// <summary>
    /// Leaves the current lobby (deletes lobby if host).
    /// </summary>
    public async void LeaveLobby()
    {
        try
        {
            if (currentLobby != null)
            {
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

    /// <summary>
    /// Returns the lobby code.
    /// </summary>
    public string GetLobbyCode()
    {
        return currentLobby != null ? currentLobby.LobbyCode : string.Empty;
    }

    /// <summary>
    /// Returns the current lobby object.
    /// </summary>
    public Lobby GetCurrentLobby()
    {
        return currentLobby;
    }

    /// <summary>
    /// Creates a player object with a name to be used with UGS.
    /// </summary>
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

    /// <summary>
    /// Checks if the current local player is the host.
    /// </summary>
    private bool IsHost()
    {
        return currentLobby != null && AuthenticationService.Instance.PlayerId == currentLobby.HostId;
    }
}
