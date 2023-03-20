﻿using FMOD.Studio;
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
    [FMODUnity.EventRef] public string CompanionFloating = "";
    [FMODUnity.EventRef] public string BoxBreak = "";

    EventInstance LevelMusic;
    //Parameter paramInstance;

    public static AudioManager Instance;

    public enum LevelMusicTracks { NONE, UpperBavelle_City, UpperBavelle_Battle }


    private EventInstance voiceoverEvent;

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

        LevelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/"+ levelTrack.ToString());
        
        LevelMusic.start();
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
        try
        {
            voiceoverEvent = RuntimeManager.CreateInstance("event:/Dialogue/" + voPath);
        }
        catch (Exception e) 
        {
            Debug.LogWarning(e.Message);
            if (callback != null) callback();
            return;
        }

        EventDescription ed;
        voiceoverEvent.getDescription(out ed);
        int length;
        ed.getLength(out length);

        voiceoverEvent.set3DAttributes(RuntimeUtils.To3DAttributes(GameManager.Instance.PlayerInstance.gameObject));
        voiceoverEvent.start();

        StartCoroutine(StopVoicelineAfter(length, callback));
        //EventReference eventVO = RuntimeManager.PathToEventReference("event:/Dialogue/" + voPath);
        //FMODUnity.RuntimeManager.PlayOneShot(eventVO, GameManager.Instance.PlayerInstance.transform.position);
    }

    private IEnumerator StopVoicelineAfter(int length, Action callback = null)
    {
        yield return new WaitForSeconds((float) length / 1000);
        StopCurrentVoiceline();
        if(callback!= null) callback();
    }

    public void StopCurrentVoiceline()
    {
        voiceoverEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        voiceoverEvent.release();
    }
}
