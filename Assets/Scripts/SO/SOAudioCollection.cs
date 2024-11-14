using System.Collections.Generic;
using UnityEngine;
using System; 

[CreateAssetMenu(menuName = "Audio/Audio Collection", order = 0)]
public class SOAudioCollection : ScriptableObject
{
    public List<SOAudioContainer> collection;

    public AudioClip GetRandomClip(int indexCollection, Enum indexList)
    {
        if (collection.Count <= 0) return null; ;

        return collection[indexCollection].GetRandomClip(indexList);
    }

    public AudioClip GetAClip(int indexCollection, int indexList, int indexClip)
    {
        if (collection.Count <= 0 || indexList >= collection.Count || indexCollection < 0) return null;

        return collection[indexCollection].GetAClip(indexList, indexClip);
    }
}