using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioTrack[] tracks;

    void Awake()
    {
        foreach(AudioTrack t in tracks){
            t.source = gameObject.AddComponent<AudioSource>();
            t.source.clip = t.clip;
            t.source.volume = t.volume;
            // t.source.pitch = t.pitch;
        }
    }

    public void Play(string trackName)
    {
        AudioTrack t = Array.Find(tracks, t => t.trackName == trackName);
        t.source.Play();
    }

    public void Stop(string trackName)
    {
        AudioTrack t = Array.Find(tracks, t => t.trackName == trackName);
        t.source.Stop();
    }
}
