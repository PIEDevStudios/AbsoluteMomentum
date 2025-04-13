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
    private void Update()
    {
        countdownValue = raceManager.GetCountdownTimer();
        if (countdownValue > 0)
        {
            countdownText.text = countdownValue.ToString();
            countdownText.enabled = true;
            hasCountdownFinished = false;
        }
        else if (countdownValue == 0 && !hasCountdownFinished)
        {
            hasCountdownFinished = true;
            OnCountdownFinished?.Invoke(); // Invoke the event
            StartCoroutine(DisplayGoCountdownCoroutine());
        }
    }

    private IEnumerator DisplayGoCountdownCoroutine()
    {
        countdownText.text = "GO!";
        
        yield return new WaitForSeconds(1f);

        countdownText.enabled = false; // Hide after countdown
    }
}
