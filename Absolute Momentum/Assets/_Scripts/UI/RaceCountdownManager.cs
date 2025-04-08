using System.Collections;
using UnityEngine;
using TMPro;
using System;
using Unity.Netcode;

// Client Side
public class RaceCountdownManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to TextMeshProUGUI
    [SerializeField] private Player player;
    public event Action OnCountdownFinished; // Event triggered when countdown finishes
    private RaceManager raceManager;
    private float countdownValue;
    private bool hasCountdownFinished;
    private bool isCountingDown = false;
    private void Start()
    {
        countdownText.enabled = false;

        if (IsOwner)
        {
            OnCountdownFinished += player.playerRaceTimeManager.StartTimer;
        }
           
        
        raceManager = RaceManager.Instance;
    }

    public void StartCountdown()
    {
        if (!isCountingDown)
        {
            StartCoroutine(CountdownRoutine());
        }
    }

    private void Update()
    {
        countdownValue = raceManager.GetCountdownTimer();
        
        if (countdownValue > 0)
        {
            if(player != null)
            {
                player.playerInput.enabled = false;
            }
            countdownText.text = countdownValue.ToString();
            countdownText.enabled = true;
            hasCountdownFinished = false;
        }
        else if (countdownValue == 0 && !hasCountdownFinished)
        {
            hasCountdownFinished = true;
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownRoutine()
    {
        isCountingDown = true;
        countdownValue = 3; // Or whatever countdown you want

        while (countdownValue > 0)
        {
            countdownText.text = countdownValue.ToString();
            countdownText.enabled = true;
            yield return new WaitForSeconds(1);
            countdownValue--;
        }

        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.text = "GO!";
        
        yield return new WaitForSeconds(1f);

        countdownText.enabled = false; // Hide after countdown

        if(player != null)
        {
            player.playerInput.enabled = true;
            float localTime = Time.realtimeSinceStartup;
            Debug.Log($"[Client {NetworkManager.Singleton.LocalClientId}] Gained control at {localTime:F3} seconds (realtime)");
        }
        
        // Keep track of this
        OnCountdownFinished?.Invoke(); // Invoke the event
    }
}
