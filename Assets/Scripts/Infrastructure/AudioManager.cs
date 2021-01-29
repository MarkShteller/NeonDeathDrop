using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public AudioSource MusicSource1;
    public AudioSource MusicSource2;
    //public AudioSource EffectSource;

    [FMODUnity.EventRef] public string MusicAct1Track1 = "";
    [FMODUnity.EventRef] public string EnvAmbienceHighway = "";
    [FMODUnity.EventRef] public string EnvTileCracked = "";

    [FMODUnity.EventRef] public string PlayerDash = "";
    [FMODUnity.EventRef] public string PlayerTakeDamage = "";
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

    [FMODUnity.EventRef] public string EnemyTakePushHit = "";
    [FMODUnity.EventRef] public string EnemyTakeDashHit = "";
    [FMODUnity.EventRef] public string EnemyFall = "";
    [FMODUnity.EventRef] public string EnemyTurretShoot = "";
    [FMODUnity.EventRef] public string EnemyBombTriggered = "";
    [FMODUnity.EventRef] public string EnemyStompingStomp = "";

    [FMODUnity.EventRef] public string UIRestart = "";


    public static AudioManager Instance;

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

    void Start ()
    {
        //PlayMusic(0);
        //placeholder
        FMODUnity.RuntimeManager.PlayOneShot(MusicAct1Track1, transform.position);

    }

    public void LoadMusicToSecondarySource(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound "+ name+" could not be found.");
            return;
        }

        if (MusicSource1.isPlaying)
        {
            MusicSource2.clip = s.clip;
            MusicSource2.volume = 0;
            MusicSource2.loop = s.loop;
            MusicSource2.Stop();
        }
        else if (MusicSource2.isPlaying)
        {
            MusicSource1.clip = s.clip;
            MusicSource1.volume = 0;
            MusicSource1.loop = s.loop;
            MusicSource1.Stop();
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " could not be found.");
            return;
        }

        MusicSource1.clip = s.clip;
        MusicSource1.Play();
        MusicSource1.loop = s.loop;
        MusicSource2.Stop();
    }

    public void PlayMusic(int index)
    {
        MusicSource1.clip = sounds[index].clip;
        MusicSource1.Play();
        MusicSource1.loop = sounds[index].loop;
        MusicSource1.volume = sounds[index].volume;
    }

    public void CrossfadeMusic(float speed)
    {
        if (MusicSource1.isPlaying)
        {
        print("## crossfading 1>2");
            MusicSource2.Play();
            StartCoroutine(Crossfade(MusicSource1, MusicSource2, speed));
        }
        else if (MusicSource2.isPlaying)
        {
        print("## crossfading 2>1");
            MusicSource1.Play();
            StartCoroutine(Crossfade(MusicSource2, MusicSource1, speed));
        }
    }

    private IEnumerator Crossfade(AudioSource sourceA, AudioSource sourceB, float speed)
    {
        float maxVolume = sourceA.volume;
        while (sourceB.volume < maxVolume)
        {
            sourceA.volume -= speed;
            sourceB.volume += speed;
            yield return new WaitForSeconds(0.1f);
        }
        print("## finished crossfading");
        sourceA.Stop();
    }

    public void PlayEffect(AudioSource source, int index)
    {
        Sound s = sounds[index];
        source.clip = s.clip;
        source.volume = s.volume;
        source.loop = s.loop;

        source.Play(0);
    }
}
