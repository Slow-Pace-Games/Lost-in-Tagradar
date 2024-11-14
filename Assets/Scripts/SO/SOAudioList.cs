using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Clips", order = 2)]
public class SOAudioList : ScriptableObject
{
    public List<AudioClip> clips;


    public AudioClip GetRandomClip()
    {
        if (clips.Count <= 0)
        {
            Debug.LogError("No clip");
            return null;
        }

        return clips[Random.Range(0, clips.Count)];
    }

    public AudioClip GetAClip(int index)
    {
        if (clips.Count <= 0 || index < 0 || index >= clips.Count)
        {
            Debug.LogError("No clip");
            return null;
        }

        return clips[index];
    }

    public List<AudioClip> GetAllClips()
    {
        return clips;
    }
}