using Unity.Cinemachine;
using UnityEngine;

public abstract class BaseDeathScreen : MonoBehaviour
{
    [SerializeField] protected float duration;
    [SerializeField, Range(0f,1f)] protected float respawnPlayerTime; // normalized time that player should spawn after start of screen
    public void Play()
    {
        gameObject.SetActive(true);
        OnPlay();
    }

    public float GetRespawnDelay()
    {
        return respawnPlayerTime * duration;
    }

    protected abstract void OnPlay();
}
