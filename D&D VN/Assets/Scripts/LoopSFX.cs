using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSFX : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;

    private void OnEnable()
    {
        source.clip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(source.clip);
    }

    private void Update()
    {
        if (!source.isPlaying)
        {
            source.clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(source.clip);
        }
    }
}
