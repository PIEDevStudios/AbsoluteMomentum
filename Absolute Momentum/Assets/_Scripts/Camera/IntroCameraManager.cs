using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IntroCameraManager : NetworkBehaviour
{
    [System.Serializable]
    public class IntroShot
    {
        public GameObject cameraObject;
        public float duration = 3f;
    }

    public List<IntroShot> introShots;
    public float delayBeforeCountdown = 1f;
    // public GameObject countdownUI;

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // Reset Race Manager Values
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Player>().playerUI.DisplayLevelPreviewUI();
        if (IsServer)
        {
            RaceManager.Instance.ResetRaceManagerValues(); // Only server resets
        }
        
        // Disable all cameras initially
        DisableAllCameras();

        // Play each intro shot
        foreach (var shot in introShots)
        {
            if (shot.cameraObject != null)
                shot.cameraObject.SetActive(true);

            yield return new WaitForSeconds(shot.duration);

            if (shot.cameraObject != null)
                shot.cameraObject.SetActive(false);
        }
        
        // Player camera should be the last camera that is active
        
        yield return new WaitForSeconds(delayBeforeCountdown);
        
        RaceManager.Instance.MarkPlayerSceneReadyServerRpc();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Player>().playerUI.DisplayInGameUI();
    }

    private void DisableAllCameras()
    {
        foreach (var shot in introShots)
        {
            if (shot.cameraObject != null)
                shot.cameraObject.SetActive(false);
        }
    }
}