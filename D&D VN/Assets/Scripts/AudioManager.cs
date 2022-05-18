using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioTrack[] tracks;

    #region Track Names
        public const string TITLE_MUSIC = "Title";
        public const string AMBIENT_MUSIC = "Ambient";
        public const string GOOD_END_MUSIC = "Good";
        public const string BAD_END_MUSIC = "Bad";
        public const string NORMAL_FIGHT_MUSIC = "Fight";
        public const string BOSS_FIGHT_MUSIC = "Boss";
        public const string INVESTIGATION_MUSIC = "Investigation";
    #endregion

    void Awake()
    {
        foreach(AudioTrack t in tracks){
            t.source = gameObject.AddComponent<AudioSource>();
            t.source.clip = t.clip;
            t.source.volume = t.volume;
            // t.source.pitch = t.pitch;
            t.source.loop = t.loop;
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

    public void StopAllTracks()
    {
        foreach(AudioTrack t in tracks){
            t.source.Stop();
        }
    }
}
