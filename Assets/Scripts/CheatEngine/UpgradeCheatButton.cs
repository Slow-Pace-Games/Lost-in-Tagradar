using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCheatButton : MonoBehaviour
{
    [Header("Upgrade Cheat")]
    [SerializeField] private Toggle unlock;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameMilestone;
    private SOTier currentUpgrade;

    public void Init(SOTier upgrade)
    {
        currentUpgrade = upgrade;

        icon.sprite = currentUpgrade.icon;
        nameMilestone.text = currentUpgrade.nameTier;

        unlock.isOn = currentUpgrade.isUnlock;
        unlock.onValueChanged.AddListener(UnlockUpgrade);
    }

    public void UpdateUI()
    {
        unlock.isOn = currentUpgrade.isUnlock;
    }

    private void UnlockUpgrade(bool value)
    {
        currentUpgrade.isUnlock = value;
    }
}