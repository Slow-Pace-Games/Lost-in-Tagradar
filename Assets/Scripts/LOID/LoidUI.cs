using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoidUI : MonoBehaviour
{
    #region Singleton
    public static LoidUI Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(AnimationController.PlayAnimation(LoidAnim, LoidImage, true, 0.025f));
            onAnimFinish += () => crt = StartCoroutine(AnimationController.PlayAnimation(voiceLineSpritesLoop, voiceLineImage, true, 0.025f));
        }
        else
            Destroy(gameObject);
    }
    #endregion

    public delegate void AnimFinishEvent();
    public AnimFinishEvent onAnimFinish;
    private Coroutine crt;

    public delegate void DialogueFinishEvent();
    public DialogueFinishEvent onDialogueFinish;

    public delegate void TutoFinishEvent();
    public TutoFinishEvent onTutoFinish;

    private float readingSpeed = 0.5f;
    private float holdTime = 2f;
    private float holdTimeVoice = 1.75f;

    [Header("Loid text dialogue")]
    [SerializeField] private GameObject loidDialogue;
    private RectTransform dialogueTransform;
    [SerializeField] private TextMeshProUGUI textDialogue;

    [Header("Loid voice line")]
    [SerializeField] private GameObject loidVoiceLine;
    [SerializeField] private Image voiceLineImage;
    [SerializeField] private Image LoidImage;
    [SerializeField] private Sprite[] voiceLineSpritesStart;
    [SerializeField] private Sprite[] voiceLineSpritesLoop;
    [SerializeField] private Sprite[] voiceLineSpritesEnd;
    [SerializeField] private Sprite[] LoidAnim;

    [Header("Tuto tips")]
    [SerializeField] private GameObject tipsTuto;
    [SerializeField] private TextMeshProUGUI tutoTipsText;

    private AudioSource audioSource;

    public void MoveDialogue(MoveDirection direction)
    {
        if (dialogueTransform == null) dialogueTransform = loidDialogue.GetComponent<RectTransform>();
        Vector2 pos = dialogueTransform.anchoredPosition;

        switch (direction)
        {
            case MoveDirection.Up:
                pos.y = 250f;
                break;

            case MoveDirection.Down:
                pos.y = 180f;
                break;
        }

        dialogueTransform.anchoredPosition = pos;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        ToggleLoidUI();
        //StartCoroutine(LoidVoiceLine(dialogue.voiceLine));//TODO
        StartCoroutine(LoidDialogue(dialogue.voiceType, dialogue.dialogue));
    }
    public void UpdateTuto(SOLoidTuto tuto)
    {
        DisableTutoTips();
        ToggleLoidUI();

        StartCoroutine(LoidVoiceLineTuto(tuto.VoiceLine));
        StartCoroutine(LoidDialogueTuto(tuto.TutoDialogues, tuto.TutoTips));
    }

    private void UpdateTutoTips(string tutoTips)
    {
        tipsTuto.SetActive(true);
        tutoTipsText.text = tutoTips;
    }
    private void DisableTutoTips() => tipsTuto.SetActive(false);

    private void ToggleLoidUI()
    {
        loidDialogue.SetActive(!loidDialogue.activeSelf);
        loidVoiceLine.SetActive(!loidVoiceLine.activeSelf);
    }

    private IEnumerator LoidVoiceLine(List<AudioClip> voiceLine)
    {
        yield return null;
    }
    private IEnumerator LoidDialogue(VoiceType voiceType, string dialogue)
    {
        textDialogue.maxVisibleCharacters = 1;
        textDialogue.text = dialogue;
        textDialogue.fontStyle = (voiceType == VoiceType.Command) ? FontStyles.Italic : FontStyles.Normal;

        while (textDialogue.maxVisibleCharacters != dialogue.Length)
        {
            yield return new WaitForSeconds(readingSpeed / 10f);
            textDialogue.maxVisibleCharacters++;
        }

        yield return new WaitForSeconds(holdTime);

        ToggleLoidUI();
        onDialogueFinish?.Invoke();
    }

    private IEnumerator LoidVoiceLineTuto(AudioClip[] voiceLine)
    {
        int indexVoiceLine = 0;
        StartCoroutine(AnimationController.PlayAnimation(voiceLineSpritesStart, voiceLineImage, false, 0.025f, onAnimFinish));
        voiceLineImage.enabled = true;

        while (voiceLine.Length != indexVoiceLine)
        {
            voiceLineImage.enabled = true;
            audioSource.clip = voiceLine[indexVoiceLine];
            audioSource.Play();
            yield return new WaitForSeconds(holdTimeVoice + audioSource.clip.length);
            indexVoiceLine++;
        }

        StopCoroutine(crt);
        StartCoroutine(AnimationController.PlayAnimation(voiceLineSpritesEnd, voiceLineImage, false, 0.025f));
        voiceLineImage.enabled = true;
    }

    private IEnumerator LoidDialogueTuto(TutoDialogue[] dialogues, string tutoTips)
    {
        int indexDialogue = 0;

        while (dialogues.Length != indexDialogue)
        {
            TutoDialogue dialogue = dialogues[indexDialogue];

            textDialogue.fontStyle = (dialogue.VoiceType == VoiceType.Command) ? FontStyles.Italic : FontStyles.Normal;
            textDialogue.maxVisibleCharacters = 1;
            textDialogue.text = dialogue.Text;

            while (textDialogue.maxVisibleCharacters != dialogue.Text.Length)
            {
                yield return new WaitForSeconds(readingSpeed / 10f);
                textDialogue.maxVisibleCharacters++;
            }

            indexDialogue++;
            yield return new WaitForSeconds(dialogue.HoldTime);
        }

        if (tutoTips != string.Empty) { UpdateTutoTips(tutoTips); }

        ToggleLoidUI();
        onTutoFinish.Invoke();
    }
}

public enum MoveDirection { Up, Down, Left, Right }