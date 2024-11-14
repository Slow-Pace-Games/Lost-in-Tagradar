using UnityEngine;

[CreateAssetMenu(menuName = "Loid/Loid Data", fileName = "Loid data")]
public class SOLoidTips : ScriptableObject
{
    [SerializeField] private AudioClip[] voiceLine;
    [SerializeField] private string[] dialogue;

    public AudioClip[] VoiceLine { get => voiceLine; }
    public string[] Dialogue { get => dialogue; }
}