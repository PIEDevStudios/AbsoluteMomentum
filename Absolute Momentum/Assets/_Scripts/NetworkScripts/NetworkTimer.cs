using UnityEngine;

public class NetworkTimer
{
    private float timer;
    public float MinTimeBetweeenTicks { get; }
    private int currentTick;

    public NetworkTimer(float serverTickRate)
    {
        MinTimeBetweeenTicks = 1f / serverTickRate;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;
    }

    public bool ShouldTick()
    {
        if (timer >= MinTimeBetweeenTicks)
        {
            timer -= MinTimeBetweeenTicks;
            currentTick++;
            return true;
        }
        
        return false;
            
    }


}
