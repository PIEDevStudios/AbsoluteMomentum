using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRaceTimeManager : NetworkBehaviour
{
    public float timer {get; private set;}
    private bool timerActive = false;
    
    public void StartTimer()
    {
        timer = 0f;
        timerActive = true;
    }

    private void Update()
    {
        if (!timerActive) return;
        
        timer += Time.deltaTime;
    }

    public void StopTimer()
    {
        timerActive = false;
        RaceManager.Instance.SubmitRaceTimeServerRPC(timer);
    }

    public void ResetTimer()
    {
        timer = 0f;
    }
}