using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip spaceClip;

    [Header("Kepler Dubb Audio")]
    public AudioSource audioSourceKeplerDubb;
    public AudioClip keplerDubbClip;
    void Start()
    {
        PlayDefaultSound();
    }

    public void PlayDefaultSound()
    {
        audioSource.clip = spaceClip;
        audioSource.Play();
    }

    public void PlaySoundKeplerLaw()
    {
        audioSourceKeplerDubb.clip = keplerDubbClip;
        audioSourceKeplerDubb.Play();
    }

    public void StopSoundKeplerLaw()
    {
        audioSource.Stop();
    }

}
