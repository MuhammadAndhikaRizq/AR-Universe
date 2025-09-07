using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip spaceClip;
    public AudioClip tableClip;

    [Header("Kepler Dubb Audio")]
    public AudioSource audioSourceKeplerDubb;
    public AudioClip keplerDubbClip;
    public AudioSource tableDubbClip;

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

    public void PlaySoundTable()
    {
        tableDubbClip.clip = tableClip;
        tableDubbClip.Play();
    }

    public void StopSoundKeplerLaw()
    {
        audioSource.Stop();
    }

}
