using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public AudioSource MusicSource1;
    public AudioSource MusicSource2;


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

        /*foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }*/
    }

    void Start ()
    {
        Play(0);
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

    public void Play(int index)
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
}
