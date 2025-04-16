using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyTrigger : NetworkBehaviour
{
    [SerializeField] private string SceneName;
    [SerializeField] private TextMeshPro LobbyText;
    [SerializeField] private TMP_Text LobbyCountdown;
    [SerializeField] private float countdown = 3f;
    private int numPlayersReady = 0;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;
        
        Debug.Log("TRIGGER" + other);
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerReady();
        }
    }

    private IEnumerator Countdown()
    {
        float currentCountdown = countdown;

        while (currentCountdown >= 0)
        {
            if (IsServer && numPlayersReady != NetworkManager.Singleton.ConnectedClientsList.Count)
            {
                LobbyCountdown.text = "";
                StopCountdownClientRpc();
                yield break;
            }

            LobbyCountdown.text = currentCountdown.ToString();
            yield return new WaitForSeconds(1);
            currentCountdown--;
        }

        LobbyCountdown.text = "";

        StartCoroutine(FadeTransition());
    }

    [ClientRpc]
    private void StopCountdownClientRpc()
    {
        StopAllCoroutines();
        LobbyCountdown.text = "";
    }

    private IEnumerator FadeTransition()
    {
        // FADE TRANSITION
        yield return new WaitForSeconds(1);
        // AudioManager.Instance.SetMusicArea(AudioManager.MusicArea.Level);
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    private void PlayerReady()
    {
        numPlayersReady++;
        UpdateLobbyTextClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClientsList.Count);

        if (numPlayersReady == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            Debug.Log("Loading Next Scene");

            CountdownClientRPC();
            // FadeTransitionClientRPC();
            // levelLoader.fadeTransition();
            // NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    private void CountdownClientRPC()
    {
        StopAllCoroutines();
        StartCoroutine(Countdown());
    }

    private void OnTriggerExit(Collider other)
    {
        if(!IsServer) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerUnreadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerUnreadyServerRpc()
    {
        numPlayersReady--;

        if(numPlayersReady < 0)
            numPlayersReady = 0;

        UpdateLobbyTextClientRpc(numPlayersReady ,NetworkManager.Singleton.ConnectedClientsList.Count);
    }
    [ClientRpc]
    private void UpdateLobbyTextClientRpc(int _playersReady, int _playersConnected)
    {
        LobbyText.text = _playersReady + " / " + _playersConnected + " Players Ready";
    }



}