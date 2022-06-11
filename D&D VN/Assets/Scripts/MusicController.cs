using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip goodEndingMusic;
    [SerializeField] private AudioClip badEndingMusic;

    public void PlayGoodEndingMusic()
    {
        source.Stop();
        source.clip = goodEndingMusic;
        source.Play();
    }

    public void PlayBadEndingMusic()
    {
        source.Stop();
        source.clip = badEndingMusic;
        source.Play();
    }

    public void StopMusic()
    {
        source.Stop();
    }
}
