using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MilestoneCheat : MonoBehaviour
{
    [Header("Milestone")]
    [SerializeField] private Button reset;
    [SerializeField] private Button unlockAll;
    [SerializeField] private SODatabase database;
    private List<SOMilestone> defaultMilestone;

    [Header("Milestone Build")]
    [SerializeField] private GameObject prefabMilstone;
    [SerializeField] private GameObject prefabUpgrade;
    [SerializeField] private RectTransform containerMilestone;
    private List<MilestoneCheatButton> milestoneCheat = new List<MilestoneCheatButton>();

    private void Start()
    {
        defaultMilestone = database.AllMilestones;
        reset.onClick.AddListener(ResetMilestone);
        unlockAll.onClick.AddListener(UnlockAllMilestone);

        BuildMilestone();
    }

    private void BuildMilestone()
    {
        for (int i = 0; i < database.AllMilestones.Count; i++)
        {
            GameObject milestone = Instantiate(prefabMilstone, containerMilestone);
            milestoneCheat.Add(milestone.GetComponent<MilestoneCheatButton>());
            milestoneCheat[i].Init(database.AllMilestones[i]);
        }
    }

    private void ResetMilestone()
    {
        for (int i = 0; i < defaultMilestone.Count; i++)
        {
            database.AllMilestones[i] = defaultMilestone[i];
        }

        UpdateUIMilestone();
    }

    private void UpdateUIMilestone()
    {
        for (int i = 0; i < milestoneCheat.Count; i++)
        {
            milestoneCheat[i].UpdateUI();
        }
    }

    private void UnlockAllMilestone()
    {
        for (int i = 0; i < database.AllMilestones.Count; i++)
        {
            database.AllMilestones[i].isUnlock = true;
            database.AllMilestones[i].isFinished = true;

            for (int j = 0; j < database.AllMilestones[i].tier.Count; j++)
            {
                for (int k = 0; k < database.AllMilestones[i].tier[j].rewards.buildingReward.Count; k++)
                {
                    database.AllMilestones[i].tier[j].rewards.buildingReward[k].isDiscovered = true;
                }
            }
        }
    }
}