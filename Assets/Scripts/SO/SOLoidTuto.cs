using UnityEngine;

[CreateAssetMenu(menuName = "Loid/Loid Tuto", fileName = "LoidTuto")]
public class SOLoidTuto : ScriptableObject
{
    [SerializeField] private AudioClip[] voiceLine;
    [SerializeField] private TutoDialogue[] tutoDialogues;
    [SerializeField, TextArea(5, 10)] private string tutoTips;
    [SerializeField] private string tutoTitle;
    [SerializeField, TextArea(5, 10)] private string tutoDescription;


    public AudioClip[] VoiceLine { get => voiceLine; }
    public TutoDialogue[] TutoDialogues { get => tutoDialogues; }
    public string TutoTips { get => tutoTips; }
    public string TutoTitle { get => tutoTitle; }
    public string TutoDescription { get => tutoDescription; }
}

[System.Serializable]
public class TutoDialogue
{
    [SerializeField, TextArea(2, 5)] private string dialogue;
    [SerializeField] private VoiceType voiceType;
    [SerializeField] private float holdTime = 2f;

    public float HoldTime { get => holdTime; }
    public string Text { get => dialogue; }
    public VoiceType VoiceType { get => voiceType; }

}