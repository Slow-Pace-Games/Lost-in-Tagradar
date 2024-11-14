using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] AudioSource sourceMusic;
    [SerializeField] AudioSource sourceAmbiance;
    float timer;
    float timerMax;

    float timerAmbiance;
    float timerMaxAmbiance;
    void Start()
    {
        timerMax = 150f;
        timer = timerMax;

        timerMaxAmbiance = 90f;
        timerAmbiance = timerMaxAmbiance;

    }

    void Update()
    {
        PlayMusic();
        PlayAmbiance();
    }

    void PlayMusic()
    {
        if (!sourceMusic.isPlaying)
        {
            timer += Time.deltaTime;
        }

        if (timer > timerMax)
        {
            timer = 0;
            sourceMusic.clip = AudioManager.Instance.GetRandomClip(AudioCollection.Track, TrackAudio.Game);
            sourceMusic.Play();
        }
    }

    void PlayAmbiance()
    {
        if (!sourceAmbiance.isPlaying)
        {
            timerAmbiance += Time.deltaTime;
        }

        if (timerAmbiance > timerMaxAmbiance)
        {
            timerAmbiance = 0;
            sourceAmbiance.clip = AudioManager.Instance.GetRandomClip(AudioCollection.Ambiance, AmbianceAudio.Wind);
            sourceAmbiance.Play();
        }
    }
}
