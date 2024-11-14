using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSound : MonoBehaviour
{
    List<AudioSource> mainAudioSource = new List<AudioSource>();
    AudioSource secondAudioSource;
    [SerializeField] AudioMixerGroup mainSFX;
    [SerializeField] AudioMixerGroup secondarySFX;

    float timerPlayerFootStep;
    float maxTimerPlayerFootStep;

    private void Start()
    {

        //instanciate main audio source
        for (int i = 0; i < 2; i++)
        {
            AudioSource go = gameObject.AddComponent<AudioSource>();
            go.outputAudioMixerGroup = mainSFX;
            mainAudioSource.Add(go);
        }

        //instanciate second audio source
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = secondarySFX;
        secondAudioSource = audioSource;

        maxTimerPlayerFootStep = 1f;
        timerPlayerFootStep = maxTimerPlayerFootStep;
    }

    private void Update()
    {
        timerPlayerFootStep += Time.deltaTime;
        if (timerPlayerFootStep > maxTimerPlayerFootStep)
        {
            timerPlayerFootStep = maxTimerPlayerFootStep;
        }
    }

    public void PlayRandomSoundInPlayerHit()
    {
        foreach (AudioSource source in mainAudioSource)
        {
            if (!source.isPlaying)
            {
                source.pitch = 1;
                source.clip = AudioManager.Instance.GetRandomClip(AudioCollection.Player, PlayerAudio.Hit);
                source.Play();
                return;
            }
        }
    }

    public void PlayRandomSoundInPlayerFootStep(bool isRunning)
    {
        if (timerPlayerFootStep == maxTimerPlayerFootStep)
        {
            timerPlayerFootStep = 0f;

            if (isRunning)
            {
                maxTimerPlayerFootStep = 0.25f;
            }
            else
            {
                maxTimerPlayerFootStep = 0.5f;
            }

            if (secondAudioSource.isPlaying)
            {
                return;
            }

            if (!secondAudioSource.isPlaying)
            {
                secondAudioSource.pitch = 1;
                secondAudioSource.pitch += Random.Range(-0.1f, 0.2f);
                secondAudioSource.clip = AudioManager.Instance.GetRandomClip(AudioCollection.Player, PlayerAudio.Walk);
                secondAudioSource.Play();
                return;
            }
        }


    }

    public void PlaySoundTaser()
    {
        foreach (AudioSource source in mainAudioSource)
        {
            if (source.isPlaying)
            {
                return;
            }
        }

        foreach (AudioSource source in mainAudioSource)
        {
            if (!source.isPlaying)
            {
                source.pitch = 1;
                source.pitch += Random.Range(-0.1f, 0.2f);
                source.clip = AudioManager.Instance.GetRandomClip(AudioCollection.Player, PlayerAudio.AttackTaser);
                source.Play();
                return;
            }
        }
    }

    public void PlayRandomSoundInPickUp()
    {
        foreach (AudioSource source in mainAudioSource)
        {
            if (source.isPlaying)
            {
                return;
            }
        }

        foreach (AudioSource source in mainAudioSource)
        {
            if (!source.isPlaying)
            {
                source.pitch = 1;
                source.clip = AudioManager.Instance.GetRandomClip(AudioCollection.Player, PlayerAudio.PickUp);
                source.Play();
                return;
            }
        }
    }
}
