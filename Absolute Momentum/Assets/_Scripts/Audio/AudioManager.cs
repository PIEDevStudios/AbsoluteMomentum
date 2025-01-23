using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonPersistent<AudioManager>
{
    // List of Banks to load
    [FMODUnity.BankRef]
    public List<string> Banks = new List<string>();

    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume;
    [Range(0, 1)]
    public float musicVolume;
    [Range(0, 1)]
    public float ambienceVolume;
    [Range(0, 1)]
    public float SFXVolume;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    public List<EventInstance> eventInstances {  get; private set; }
    public List<StudioEventEmitter> eventEmitters { get; private set; }

    public EventInstance ambienceEventInstance;
    public EventInstance musicEventInstance;



    protected override void Awake()
    {        
        base.Awake();

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        UpdateVolume();

   
    }

    private void Start()
    {

    }

    private void UpdateVolume()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("Area", (float)area);
    }

    public void PlayOneShot(FMODEvents.Sounds sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(FMODEvents.Instance.SfxArray[(int)sound], worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(FMODEvents.Sounds sound, GameObject emitterGameObject)
    {
        if (emitterGameObject.TryGetComponent(out StudioEventEmitter emitter))
        {
            emitter.Stop();
            eventEmitters.Add(emitter);
        }
        else
        {
            emitter = emitterGameObject.AddComponent<StudioEventEmitter>();
        }
        emitter.EventReference = FMODEvents.Instance.SfxArray[(int)sound];
        emitter.AllowFadeout = true;
        
        return emitter;
    }

    public void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }


    public enum MusicArea
    {
        MaeTheme,
        Escape,
        Boss
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolume();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolume();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        UpdateVolume();
    }
    
    
    // CODE TO FIX BANK ISSUE
    
    
    //public void Start()
    //{
    //    StartCoroutine(WaitForLoadBanks());
    //}

    //private IEnumerator WaitForLoadBanks()
    //{
    //    foreach (var bank in Banks)
    //    {
    //        RuntimeManager.LoadBank(bank, true);
    //    }
        

    //    while (!RuntimeManager.HaveAllBanksLoaded)
    //    {
    //        yield return null;
    //    }

    //    InitializeAmbience(FMODEvents.Instance.Ambience);
    //    InitializeMusic(FMODEvents.Instance.Music);
    //}
}

