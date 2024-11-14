using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    private static AudioManager instance;
    public static AudioManager Instance { get => instance; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Database Audio")]
    [SerializeField] private SOAudioCollection sounds;
    public AudioClip GetRandomClip(AudioCollection indexCollection, Enum indexList)
    {
        return sounds.GetRandomClip((int)indexCollection, indexList);
    }
    public AudioClip GetAClip(AudioCollection indexCollection, Enum indexList, int indexClip)
    {
        return sounds.GetAClip((int)indexCollection, Convert.ToInt32(indexList), indexClip);
    }
}

public enum AudioCollection
{
    Player,
    Dindonstre,
    Kamilaka,
    Track,
    Ambiance,
}

#region Player
public enum PlayerAudio
{
    Hit,
    AttackTaser,
    Walk,
    Jump,
    Scanner,
    PickUp,
    AttackGun,
}
#endregion

#region Monstre
public enum DindonstreAudio
{
    Attack,
    Hit,
}
public enum KamilakaAudio
{
}
#endregion

#region Machine
public enum MachineAudio
{
    Ambiance,
    Work,
    Finish,
}
#endregion

#region Track
public enum TrackAudio
{
    Game,
    Menu,
}
#endregion

#region Ambiance
public enum AmbianceAudio
{
    Wind,
}
#endregion