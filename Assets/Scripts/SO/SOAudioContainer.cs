using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Container", order = 1)]
public class SOAudioContainer : ScriptableObject
{
    public List<SOAudioList> audioLists;

    public AudioClip GetRandomClip(Enum index)
    {
        int indexList = Convert.ToInt32(index);
        if (audioLists.Count <= 0 || indexList < 0 || indexList >= audioLists.Count) return null; ;

        return audioLists[indexList].GetRandomClip();
    }

    public AudioClip GetAClip(int indexList, int indexClip)
    {
        if (audioLists.Count <= 0 || indexList >= audioLists.Count || indexList < 0) return null;

        return audioLists[indexList].GetAClip(indexClip);
    }

    public List<AudioClip> GetAllClips(Enum index)
    {
        return audioLists[Convert.ToInt32(index)].GetAllClips();
    }
}