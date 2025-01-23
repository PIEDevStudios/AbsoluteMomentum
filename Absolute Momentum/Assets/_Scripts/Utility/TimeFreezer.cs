using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFreezer : Singleton<TimeFreezer>
{
    [SerializeField] private float timeScale = 0.05f;    // How slow the game slows down to
    [SerializeField] private int restoreSpeed = 10;     // Speed at which time scale is restored
    [SerializeField] private float delay = 0.1f;        // How much time before time begins to restore to normal
    private float speed;
    private bool restoreTime;



    private void Update()
    {
        // If time is restoring
        if (restoreTime)
        {
            if (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * speed;
            }
            else
            {
                Time.timeScale = 1f;
                restoreTime = false;
            }
        }
    }

    public void StopTime()
    {
        StopTime(timeScale, restoreSpeed, delay);
    }

    public void StopTime(float changeTimeScale, int restoreSpeed, float delay)
    {
        speed = restoreSpeed;

        if (delay > 0)
        {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }
        else
        {
            restoreTime = true;
        }

        Time.timeScale = changeTimeScale;
    }

    private IEnumerator StartTimeAgain(float amt)
    {
        yield return new WaitForSecondsRealtime(amt);
        restoreTime = true;
    }

}
