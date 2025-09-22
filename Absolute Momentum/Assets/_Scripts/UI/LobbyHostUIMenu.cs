using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHostUIMenu : MonoBehaviour
{
    private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private TMP_Dropdown maxPlayers;
    [SerializeField] private Toggle privateLobby;
    // [SerializeField] private TMP_Dropdown map;
    // [SerializeField] private TMP_Dropdown gameMd;
    [SerializeField] private GameObject lobbyGameUI;
    [SerializeField] private TMP_Dropdown level;


    // This exists for testing
    [SerializeField] private GameObject lobbyUI;

    private void Start()
    {
        lobbyAPI = LobbyAPI.Instance;
    }
    public void HostLobby()
    {
        lobbyAPI.CreateLobby(lobbyName.text, 
                            //  int.Parse(maxPlayers.options[maxPlayers.value].text),
                             8,
                             privateLobby.isOn);

        lobbyName.text = "";
        this.gameObject.SetActive(false);
        lobbyGameUI.SetActive(true);
        RaceManager.Instance.SetRaceScene(level.value);
    
    }

    public void BackLobby()
    {
        lobbyUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
