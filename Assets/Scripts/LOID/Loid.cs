using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Loid : MonoBehaviour
{
    #region Singleton
    public static Loid Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    #endregion

    #region Enum
    private enum LoidState
    {
        Wait,
        Speak,
    }
    private enum TutoPhase
    {
        Ship,
        Scan,
        Digg,
        Hub,
        Milestone,
        Drill,
        Electricity,
        Furnace,
        Convey,
        Destruct,
        Codex,
        Finish,
    }
    #endregion

    [Header("Loid")]
    [SerializeField] private SOLoidDatabase loidDatabase;
    [SerializeField] private TutoPhase tutoPhase;
    private LoidState state = LoidState.Wait;

    private List<DialogueData> dialogueQueue = new List<DialogueData>();
    private List<TutoPhase> tutoQueue = new List<TutoPhase>();


    private void Start()
    {
        LoidUI.Instance.onDialogueFinish += UpdateDialogueQueue;
        LoidUI.Instance.onTutoFinish += UpdateTutoQueue;

        LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
        state = LoidState.Speak;
    }

    #region Save/Load
    public int GetTutoPhase()
    {
        return (int)tutoPhase;
    }
    public void SetTutoPhase(int tutoPhase)
    {
        this.tutoPhase = (TutoPhase)tutoPhase;
    }
    #endregion

    #region Queue
    private void UpdateDialogueQueue()
    {
        state = LoidState.Wait;
        if (dialogueQueue.Count > 0)
        {
            LoidUI.Instance.StartDialogue(dialogueQueue[0]);
            state = LoidState.Speak;
        }
    }
    private void UpdateTutoQueue()
    {
        state = LoidState.Wait;
        if (tutoQueue.Count > 0)
        {
            tutoPhase = tutoQueue[0];
            tutoQueue.RemoveAt(0);
            LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
            state = LoidState.Speak;
        }
    }
    #endregion

    #region Tuto
    private bool TryLaunchTuto(TutoPhase nextPhase)
    {
        if (state == LoidState.Speak)
        {
            tutoQueue.Add(nextPhase);
            tutoPhase = nextPhase;
            return false;
        }

        state = LoidState.Speak;
        return true;
    }
    public void UpdateTuto(PlayerAction action)
    {
        if (tutoPhase == TutoPhase.Finish) return;

        if (tutoPhase == TutoPhase.Ship && action == PlayerAction.OpenShip)
        {
            if (TryLaunchTuto(TutoPhase.Scan))
            {
                tutoPhase = TutoPhase.Scan;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
            }
            return;
        }

        if (tutoPhase == TutoPhase.Scan && action == PlayerAction.Scan)
        {
            if (TryLaunchTuto(TutoPhase.Digg))
            {
                tutoPhase = TutoPhase.Digg;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
            }
            return;
        }

        if (tutoPhase == TutoPhase.Digg && action == PlayerAction.Digg)
        {
            if (TryLaunchTuto(TutoPhase.Hub))
            {
                tutoPhase = TutoPhase.Hub;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Hub && action == PlayerAction.PlaceHub)
        {
            if (TryLaunchTuto(TutoPhase.Milestone))
            {
                tutoPhase = TutoPhase.Milestone;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Milestone && action == PlayerAction.PurshaseMilestone)
        {
            if (TryLaunchTuto(TutoPhase.Drill))
            {
                tutoPhase = TutoPhase.Drill;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Drill && action == PlayerAction.PlaceDrill)
        {
            if (TryLaunchTuto(TutoPhase.Electricity))
            {
                Player.Instance.AddScannableResource(2);
                tutoPhase = TutoPhase.Electricity;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Electricity && action == PlayerAction.PlaceGenerator)
        {
            if (TryLaunchTuto(TutoPhase.Furnace))
            {
                tutoPhase = TutoPhase.Furnace;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Furnace && action == PlayerAction.PlaceFurnace)
        {
            if (TryLaunchTuto(TutoPhase.Convey))
            {
                tutoPhase = TutoPhase.Convey;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Convey && action == PlayerAction.PlaceConvey)
        {
            if (TryLaunchTuto(TutoPhase.Destruct))
            {
                tutoPhase = TutoPhase.Destruct;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Destruct && action == PlayerAction.Destruction)
        {
            if (TryLaunchTuto(TutoPhase.Codex))
            {
                tutoPhase = TutoPhase.Codex;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }

        if (tutoPhase == TutoPhase.Codex && action == PlayerAction.OpenCodex)
        {
            if (TryLaunchTuto(TutoPhase.Finish))
            {
                tutoPhase = TutoPhase.Finish;
                LoidUI.Instance.UpdateTuto(loidDatabase.Tuto[(int)tutoPhase]);
                return;
            }
        }
    }
    #endregion

    #region Dialogue
    public void LoidUpdate(VoiceType voiceType, DialogueType dialogueType, LoidData data)
    {
        if (tutoPhase != TutoPhase.Finish) return;

        DialogueData newDialogue = CreateDialogueData(dialogueType, data);
        newDialogue.voiceType = voiceType;

        TryLaunchDialogue(newDialogue);
    }
    private DialogueData CreateDialogueData(DialogueType dialogueType, LoidData data)
    {
        DialogueData dialogue = new DialogueData();

        switch (dialogueType)
        {
            case DialogueType.Resource:
                dialogue.dialogue = DialogueResource(data.newDiscovery.NameItem, data.newDiscovery.description);
                //dialogue.voiceLine = VoiceLineResource(data.newDiscovery.clip);
                break;

            case DialogueType.Mob:
                dialogue.dialogue = DialogueMob(data.monsterName);
                //dialogue.voiceLine = VoiceLineMob(loidClips[0].Clips[0]); //TODO
                break;

            case DialogueType.Ship:
                dialogue.dialogue = DialogueShip();//TODO
                //dialogue.voiceLine = VoiceLineShip();//TODO
                break;

            case DialogueType.RepairShip:
                dialogue.dialogue = DialogueRepairShip(data.shipPartName);
                //dialogue.voiceLine = VoiceLineRepairShip(loidClips[0].Clips[0]);//TODO
                break;

            case DialogueType.Milestone:
                dialogue.dialogue = DialogueMilestone(data.newMilestone.nameMilestone);
                //dialogue.voiceLine = VoiceLineMilestone(loidClips[0].Clips[0]);//TODO
                break;

            case DialogueType.Scan:
                dialogue.dialogue = DialogueScan(data.newScan.NameItem);
                //dialogue.voiceLine = VoiceLineScan(data.newScan.clip);
                break;

            default:
                break;
        }

        return dialogue;
    }
    private void TryLaunchDialogue(DialogueData newDialogue)
    {
        if (state == LoidState.Speak)
        {
            dialogueQueue.Add(newDialogue);
        }
        else
        {
            LoidUI.Instance.StartDialogue(newDialogue);
            state = LoidState.Speak;
        }
    }

    #region Dialogue/VoiceLine
    private string DialogueResource(string resourceName, string description)
    {
        string finalDialogue = string.Empty;

        finalDialogue += loidDatabase.Tips[(int)DialogueType.Resource].Dialogue[0];
        finalDialogue += resourceName;
        finalDialogue += loidDatabase.Tips[(int)DialogueType.Resource].Dialogue[1];
        finalDialogue += loidDatabase.Tips[(int)DialogueType.Resource].Dialogue[2];
        finalDialogue += description;
        finalDialogue += loidDatabase.Tips[(int)DialogueType.Resource].Dialogue[3];

        return finalDialogue;
    }
    private List<AudioClip> VoiceLineResource(AudioClip resourceName)
    {
        return new List<AudioClip>()
        {
            loidDatabase.Tips[(int)DialogueType.Resource].VoiceLine[0],
            resourceName,
            loidDatabase.Tips[(int)DialogueType.Resource].VoiceLine[1],
        };
    }

    private string DialogueMob(string mobName)
    {
        string finalDialogue = string.Empty;

        finalDialogue += loidDatabase.Tips[(int)DialogueType.Mob].Dialogue[0];
        finalDialogue += mobName;
        finalDialogue += loidDatabase.Tips[(int)DialogueType.Mob].Dialogue[1];

        return finalDialogue;
    }
    private List<AudioClip> VoiceLineMob(AudioClip mobName)
    {
        return new List<AudioClip>()
        {
            loidDatabase.Tips[(int)DialogueType.Mob].VoiceLine[0],
            mobName,
            loidDatabase.Tips[(int)DialogueType.Mob].VoiceLine[1],
        };
    }

    private string DialogueShip()
    {
        string finalDialogue = string.Empty;

        finalDialogue += loidDatabase.Tips[(int)DialogueType.Ship].Dialogue[0];

        return finalDialogue;
    }
    private List<AudioClip> VoiceLineShip()
    {
        return loidDatabase.Tips[(int)DialogueType.Ship].VoiceLine.ToList();
    }

    private string DialogueRepairShip(string partShipReapairName)
    {
        string finalDialogue = string.Empty;

        finalDialogue += loidDatabase.Tips[(int)DialogueType.RepairShip].Dialogue[0];
        finalDialogue += partShipReapairName;
        finalDialogue += loidDatabase.Tips[(int)DialogueType.RepairShip].Dialogue[1];

        return finalDialogue;
    }
    private List<AudioClip> VoiceLineRepairShip(AudioClip partShipReapairName)
    {
        return new List<AudioClip>()
        {
            loidDatabase.Tips[(int)DialogueType.RepairShip].VoiceLine[0],
            partShipReapairName,
            loidDatabase.Tips[(int)DialogueType.RepairShip].VoiceLine[1],
        };
    }

    private string DialogueMilestone(string milestoneName)
    {
        string finalDialogue = string.Empty;

        finalDialogue += loidDatabase.Tips[(int)DialogueType.Milestone].Dialogue[0];
        finalDialogue += milestoneName;
        finalDialogue += loidDatabase.Tips[(int)DialogueType.Milestone].Dialogue[1];

        return finalDialogue;
    }
    private List<AudioClip> VoiceLineMilestone(AudioClip milestoneName)
    {
        return new List<AudioClip>()
        {
            loidDatabase.Tips[(int)DialogueType.Milestone].VoiceLine[0],
            milestoneName,
        };
    }

    private string DialogueScan(string resourceScanName)
    {
        string finalDialogue = string.Empty;

        finalDialogue += loidDatabase.Tips[(int)DialogueType.Scan].Dialogue[0];
        finalDialogue += resourceScanName;
        finalDialogue += loidDatabase.Tips[(int)DialogueType.Scan].Dialogue[1];

        return finalDialogue;
    }
    private List<AudioClip> VoiceLineScan(AudioClip resourceScanName)
    {
        return new List<AudioClip>()
        {
            loidDatabase.Tips[(int)DialogueType.Scan].VoiceLine[0],
            resourceScanName,
        };
    }
    #endregion
    #endregion
}

public struct DialogueData
{
    public VoiceType voiceType;
    public List<AudioClip> voiceLine;
    public string dialogue;
}

public class LoidData
{
    public readonly SOItems newDiscovery;
    public readonly SOItems newScan;
    public readonly SOMilestone newMilestone;
    public readonly string shipPartName;
    public readonly string monsterName;

    public LoidData(SOItems soItem, bool isForScan)
    {
        if (isForScan)
            newScan = soItem;
        else
            newDiscovery = soItem;
    }
    public LoidData(SOMilestone milestoneUnlock)
    {
        newMilestone = milestoneUnlock;
    }
    public LoidData(string name, bool isForSip)
    {
        if (isForSip)
            shipPartName = name;
        else
            monsterName = name;
    }
}

public enum DialogueType
{
    Resource,
    Mob,
    Ship,
    RepairShip,
    Milestone,
    Scan,
}

public enum VoiceType
{
    Command,
    VoiceLine,
}

public enum PlayerAction
{
    OpenShip,
    Scan,
    Digg,
    PlaceHub,
    PurshaseMilestone,
    PlaceDrill,
    PlaceGenerator,
    PlaceFurnace,
    PlaceConvey,
    OpenCodex,
    Destruction,
}