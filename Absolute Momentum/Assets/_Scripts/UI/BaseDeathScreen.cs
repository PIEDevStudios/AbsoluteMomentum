using UnityEngine;

public abstract class BaseDeathScreen : MonoBehaviour
{
    public void Play()
    {
        gameObject.SetActive(true);
        OnPlay();
    }

    protected abstract void OnPlay();
}
