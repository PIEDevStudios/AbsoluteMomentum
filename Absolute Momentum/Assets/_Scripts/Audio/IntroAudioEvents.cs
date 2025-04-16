using UnityEngine;
using UnityEngine.Serialization;

public class IntroAudioEvents : Singleton<IntroAudioEvents>
{
    [SerializeField] private AudioSource[] audioSources;

    public void PlayAudio(int index)
    {
        audioSources[index].Play();
    }
    
    public void StopAllAudio()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }
    
}
