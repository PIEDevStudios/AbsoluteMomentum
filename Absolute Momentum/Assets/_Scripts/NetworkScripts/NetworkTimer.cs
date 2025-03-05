using UnityEngine;

public class NetworkTimer
{
    private float timer;
    public float MinTimeBetweeenTicks { get; }
    public int CurrentTick;

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
            CurrentTick++;
            return true;
        }
        
        return false;
            
    }


}
