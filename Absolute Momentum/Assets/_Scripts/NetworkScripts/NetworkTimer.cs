using UnityEngine;

// Networkside
public class NetworkTimer
{
    private float timer;
    public float MinTimeBetweenTicks { get; }
    public int CurrentTick;
    public float deltaTime;
    private float timeOfLastTick;

    public NetworkTimer(float serverTickRate)
    {
        MinTimeBetweenTicks = 1f / serverTickRate;
    }

    public void Update(float delta)
    {
        timer += delta;
    }

    public bool ShouldTick()
    {
        if (timer >= MinTimeBetweenTicks)
        {
            timer -= MinTimeBetweenTicks;
            deltaTime = Time.time - timeOfLastTick;
            timeOfLastTick = Time.time;
            CurrentTick++;
            return true;
        }
        
        return false;
            
    }


}
