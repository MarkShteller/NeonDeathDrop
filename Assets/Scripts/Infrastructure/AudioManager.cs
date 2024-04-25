using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public EventReference MusicAct1Track1;
    public FMODUnity.EventReference EnvAmbienceHighway;
    [FMODUnity.EventRef] public string EnvTileCracked = "";

    [FMODUnity.EventRef] public string PlayerDash = "";
    [FMODUnity.EventRef] public string PlayerTakeDamage = "";
    [FMODUnity.EventRef] public string PlayerLowEnergy = "";
    [FMODUnity.EventRef] public string PlayerFall = "";
    [FMODUnity.EventRef] public string PlayerFallLand = "";
    [FMODUnity.EventRef] public string PlayerDeath = "";
    [FMODUnity.EventRef] public string PlayerItemPickup = "";
    [FMODUnity.EventRef] public string PlayerItemPickupHP1 = "";
    [FMODUnity.EventRef] public string PlayerItemPickupHP2 = "";
    [FMODUnity.EventRef] public string PlayerItemPickupCore = "";
    [FMODUnity.EventRef] public string PlayerPush = "";
    [FMODUnity.EventRef] public string PlayerPushCombo = "";
    [FMODUnity.EventRef] public string PlayerMakeHole = "";
    [FMODUnity.EventRef] public string PlayerShockwave = "";
    [FMODUnity.EventRef] public string PlayerAOERepel = "";
    [FMODUnity.EventRef] public string PlayerSomersault = "";
    [FMODUnity.EventRef] public string PlayerSprinting = "";
    [FMODUnity.EventRef] public string PlayerSlomoEnter = "";
    [FMODUnity.EventRef] public string PlayerSlomoExit = "";

    [FMODUnity.EventRef] public string EnemyTakePushHit = "";
    [FMODUnity.EventRef] public string EnemyTakeDashHit = "";
    [FMODUnity.EventRef] public string EnemyFall = "";
    [FMODUnity.EventRef] public string EnemyTurretShoot = "";
    [FMODUnity.EventRef] public string EnemyBombTriggered = "";
    [FMODUnity.EventRef] public string EnemyStompingStomp = "";

    [FMODUnity.EventRef] public string UIRestart = "";
    public FMODUnity.EventReference VictoryFanfare;
    [FMODUnity.EventRef] public string CompanionFloating = "";
    [FMODUnity.EventRef] public string BoxBreak = "";
    public EventReference AreaUnlock;

    EventInstance LevelMusic;
    EventInstance LevelAmbiance;
    //Parameter paramInstance;

    public static AudioManager Instance;

    public enum LevelMusicTracks { NONE, UpperBavelle_City, UpperBavelle_Battle, Raheem_Battle }


    private EventInstance voiceoverEvent;
    private Action currentVOCallback;
    private Coroutine voCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //PlayMusic(0);
        //placeholder

        print("## created music instance");
        //StartCoroutine(StartMusic());

        //FMODUnity.RuntimeManager.PlayOneShot(MusicAct1Track1, transform.position);

    }

    public IEnumerator StartMusic(LevelMusicTracks levelTrack)
    {
        yield return null;
        
        //LevelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/UpperBavelle_Battle");
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(LevelMusic, player, player.GetComponent<Rigidbody>());
        print("## setting music param and playing");
        //LevelMusic.setParameterByName("level_isStarting", 1);
        
        PLAYBACK_STATE playState;
        LevelMusic.getPlaybackState(out playState);
        if (playState == PLAYBACK_STATE.PLAYING)
            LevelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        if(levelTrack == LevelMusicTracks.Raheem_Battle)
            LevelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Raheem_Battle Theme");
        else
            LevelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/"+ levelTrack.ToString());
        
        LevelMusic.start();
    }

    public void StartAmbiance(EventReference ambianceSound)
    {
        LevelAmbiance = FMODUnity.RuntimeManager.CreateInstance(ambianceSound);
        LevelAmbiance.start();
    }

    public void SetLowIntensityMusic()
    {
        //print("music low intensity");
        LevelMusic.setParameterByName("Music_Intensity", 0);
    }

    public void SetHighIntensityMusic()
    {
        //print("music high intensity");
        LevelMusic.setParameterByName("Music_Intensity", 1);

    }


    public void PlayVoiceline(string voPath, Action callback = null)
    {
        if (voCoroutine != null)
        {
            print("stopping current vo line");
            StopCoroutine(voCoroutine);
            StopCurrentVoiceline();
            currentVOCallback = null;
            voCoroutine = null;
        }

        try
        {
            voiceoverEvent = RuntimeManager.CreateInstance("event:/Dialogue/" + voPath);
        }
        catch (Exception e) 
        {
            Debug.LogWarning(e.Message);
            return;
        }

        EventDescription ed;
        voiceoverEvent.getDescription(out ed);
        int length;
        ed.getLength(out length);

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(voiceoverEvent, GameManager.Instance.PlayerInstance.transform);
        voiceoverEvent.start();

        print("playing vo line: "+ voPath + " length: "+length);
        currentVOCallback = callback;
        voCoroutine = StartCoroutine(StopVoicelineAfter(length));
    }

    internal void StopMusic()
    {
        LevelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        LevelMusic.release();
    }

    private IEnumerator StopVoicelineAfter(int length)
    {
        yield return new WaitForSeconds((float) length / 1000);
        StopCurrentVoiceline();
        if(currentVOCallback != null) currentVOCallback();
        voCoroutine = null;
    }

    public void StopCurrentVoiceline()
    {
        voiceoverEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        voiceoverEvent.release();
    }

    public void StopAllSounds()
    {
        StopCurrentVoiceline();
        LevelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        LevelAmbiance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        if(GameManager.Instance != null)
            GameManager.Instance.companion.StopFloatSound();
        LevelMusic.release();
    }
}
