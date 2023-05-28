using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioSource radioAudioSource;
    [SerializeField]
    AudioSource ambientSoundAudioSource;    
    [SerializeField]
    AudioSource endOfTheWorldAudio;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayAudio(AudioClip clip)
    {
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void PlayAudio(AudioClip clip, float delay)
    {
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.PlayDelayed(delay);
    }
    public void PlayAudioLoop(AudioClip clip)
    {
        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
        radioAudioSource.Stop();
    }

    public void StopAllAudio()
    {
        ambientSoundAudioSource.volume = 0;
        audioSource.Stop();
        radioAudioSource.Stop();
    }

    public void PlayRadio(AudioClip clip)
    {
        radioAudioSource.clip = clip;
        float delay = audioSource.clip.length;
        radioAudioSource.PlayDelayed(delay);
    }

    public void EndOfTheWorld()
    {
        endOfTheWorldAudio.Play();
    }
}
