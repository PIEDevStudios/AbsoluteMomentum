using TMPro;
using UnityEngine;

public class LobbyUIItem : MonoBehaviour
{
    public string lobbyName = "";
    public int currentPlayers = 0;
    public int maxPlayers = 0;
    public string lobbyID = "";
    public LobbyAPI lobbyAPI;

    // UI Elements to disable
    public GameObject lobbyUI;
    public GameObject lobbyGameUI;

    [SerializeField] private TMP_Text lobbyNameUI;
    [SerializeField] private TMP_Text playerCountUI;
    [SerializeField] private GameObject joinButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        lobbyNameUI.text = lobbyName;
        playerCountUI.text = currentPlayers + "/" + maxPlayers;

        joinButton.SetActive(currentPlayers < maxPlayers);
    }

    public void JoinGame()
    {
        lobbyAPI.JoinLobbyById(lobbyID);

        lobbyUI.SetActive(false);
        lobbyGameUI.SetActive(true);
    }
}
