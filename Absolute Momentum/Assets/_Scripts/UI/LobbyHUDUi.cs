using TMPro;
using UnityEngine;

public class LobbyHUDUi : MonoBehaviour
{
    private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_Text lobbyCodeText;

    private void Start()
    {
        lobbyAPI = LobbyAPI.Instance;
    }
    void Update()
    {
        lobbyCodeText.text = lobbyAPI.GetLobbyCode();
    }
}
