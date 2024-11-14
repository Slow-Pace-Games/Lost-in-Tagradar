using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MilestoneCheatButton : MonoBehaviour
{
    [Header("Milestone")]
    [SerializeField] private Toggle unlock;
    [SerializeField] private TextMeshProUGUI nameMilestone;
    private SOMilestone currentMilestone;

    [Header("Upgrade")]
    [SerializeField] private RectTransform containerUpgrade;
    [SerializeField] private GameObject prefabUpgradeCheat;
    private List<UpgradeCheatButton> upgradesCheat = new List<UpgradeCheatButton>();

    public void Init(SOMilestone milestone)
    {
        currentMilestone = milestone;
        nameMilestone.text = currentMilestone.nameMilestone;

        unlock.isOn = currentMilestone.isUnlock;
        unlock.onValueChanged.AddListener(UnlockMilestone);

        BuildUpgradeCheat();
    }

    private void BuildUpgradeCheat()
    {
        for (int i = 0; i < currentMilestone.tier.Count; i++)
        {
            GameObject upgrade = Instantiate(prefabUpgradeCheat, containerUpgrade);
            upgradesCheat.Add(upgrade.GetComponent<UpgradeCheatButton>());
            upgradesCheat[i].Init(currentMilestone.tier[i]);
        }
    }
    private void UnlockMilestone(bool value)
    {
        currentMilestone.isUnlock = value;
    }

    public void UpdateUI()
    {
        unlock.isOn = currentMilestone.isUnlock;

        for (int i = 0; i < upgradesCheat.Count; i++)
        {
            upgradesCheat[i].UpdateUI();
        }
    }
}