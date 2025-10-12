using UnityEngine;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private Slider slider;

    public void UpdateVolume()
    {
        masterGroup.audioMixer.SetFloat("Volume", slider.value);
    }   
}
