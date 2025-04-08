using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
/// <summary>
/// Manages navigation around the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject hostLobbyMenu;
    [SerializeField] private GameObject lobbyHud;
    [SerializeField] private GameObject lobbyStartTrigger;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TMP_InputField joinCodeTextInput;

    private void Awake()
    {
        // If player is already connected (returning from a race) do NOT load menu UI
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
        {
            mainMenu.SetActive(false);
            lobbyHud.SetActive(true);
            return;
        }
    }
    public void OpenLobbyMenu()
    {
        lobbyMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void CloseLobbyMenu()
    {
        lobbyMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenHostLobbyMenu()
    {
        hostLobbyMenu.SetActive(true);
        lobbyMenu.SetActive(false);
    }
    public void CloseHostLobbyMenu()
    {
        hostLobbyMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
