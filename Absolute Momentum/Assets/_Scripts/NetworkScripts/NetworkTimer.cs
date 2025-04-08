using UnityEngine;

// Networkside
public class NetworkTimer
{
    private float timer;
    public float MinTimeBetweenTicks { get; }
    public int CurrentTick;

    public NetworkTimer(float serverTickRate)
    {
        MinTimeBetweenTicks = 1f / serverTickRate;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;
    }

    public bool ShouldTick()
    {
        if (timer >= MinTimeBetweenTicks)
        {
            timer -= MinTimeBetweenTicks;
            CurrentTick++;
            return true;
        }
        
        return false;
            
    }


}
