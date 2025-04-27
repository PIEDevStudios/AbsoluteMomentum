using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUIMenu : MonoBehaviour
{
    
    private LobbyAPI lobbyAPI;

    [SerializeField] private GameObject lobbyHolder;
    [SerializeField] private GameObject lobbyItemUI;
    
    [SerializeField] private GameObject hostGameUI;
    [SerializeField] private GameObject lobbyGameUI;
    [SerializeField] private float autoRefreshTime = 10f;
    private float refreshTimeCD;

    [SerializeField] private TMP_InputField joinCodeInput;

    public void Start()
    {
        lobbyAPI = LobbyAPI.Instance;
        lobbyAPI.LobbiesUpdated += UpdateLobbyListClient;
        refreshTimeCD = autoRefreshTime;
    }

    void Update()
    {
        AutoRefreshLobby();
    }

    // void OnEnable()
    // {
    //     Debug.Log(lobbyAPI);
    //     lobbyAPI.ListLobbies();
    // }

    private void AutoRefreshLobby()
    {
        refreshTimeCD -= Time.deltaTime;

        if (refreshTimeCD < 0)
        {
            lobbyAPI.ListLobbies();
            refreshTimeCD = autoRefreshTime;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        lobbyAPI.LobbiesUpdated -= UpdateLobbyListClient;
    }

    public void HostGameUI()
    {
        hostGameUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void StartMatch()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void UpdateLobbyListClient(List<Lobby> lobbies)
    {
        // this foreach feels a bit costly
        foreach (Transform child in lobbyHolder.transform)
        {
            if (child.CompareTag("LobbyItem"))
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Lobby lobby in lobbies)
        {
            BuildLobbyItem(lobby.Name, lobby.Players.Count, lobby.MaxPlayers, lobby.Id);
        }
    }

    public void JoinWithCode()
    {
        string joinCode = joinCodeInput.text;

        lobbyAPI.OnLobbyJoined += OnLobbyJoinedHandler;
        lobbyAPI.JoinLobbyByCode(joinCode);
    }

    private void OnLobbyJoinedHandler(Lobby lobby)
    {
        joinCodeInput.text = "";
        gameObject.SetActive(false);
        lobbyGameUI.SetActive(true);

        // Unsubscribe from the event to prevent memory leaks
        lobbyAPI.OnLobbyJoined -= OnLobbyJoinedHandler;
    }

    private void BuildLobbyItem(string lobbyName, int currPlayers, int maxPlayers, string lobbyID)
    {
        GameObject currLobbyItem = Instantiate(lobbyItemUI, lobbyHolder.transform);

        LobbyUIItem lobbyItem = currLobbyItem.GetComponent<LobbyUIItem>();

        lobbyItem.lobbyName = lobbyName;
        lobbyItem.currentPlayers = currPlayers;
        lobbyItem.maxPlayers = maxPlayers;
        lobbyItem.lobbyID = lobbyID;
        Debug.Log("Lobby ID: " + lobbyID);

        lobbyItem.lobbyAPI = lobbyAPI;
        lobbyItem.lobbyUI = this.gameObject;
        lobbyItem.lobbyGameUI = lobbyGameUI;
    }

}
