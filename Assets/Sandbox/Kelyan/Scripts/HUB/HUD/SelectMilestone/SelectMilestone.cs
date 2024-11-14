using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectMilestone : MonoBehaviour
{
    #region Class
    [System.Serializable]
    private class HoverUI
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private GameObject valueContainer;
        private TextMeshProUGUI value;

        public void UpdateUI(Sprite icon, string name)
        {
            Enable();
            DisableValue();

            this.icon.sprite = icon;
            this.name.text = name;
        }

        public void UpdateUI(Sprite icon, string name, int value)
        {
            Enable();
            EnableValue();

            if (this.value == null) this.value = valueContainer.GetComponentInChildren<TextMeshProUGUI>();

            this.icon.sprite = icon;
            this.name.text = name;
            this.value.text = value.ToString();
        }

        public void Disable() => container.SetActive(false);
        private void DisableValue() => valueContainer.SetActive(false);
        private void EnableValue() => valueContainer.SetActive(true);
        private void Enable() => container.SetActive(true);
    }
    #endregion

    [Header("Data")]
    [SerializeField] private SODatabase database;
    private List<SOMilestone> milestones;
    [SerializeField] private int currenMilestone = -1;
    [SerializeField] private int currentTierUpgrade = -1;

    [Header("UI")]
    [SerializeField] private List<RewardImage> rewardTier;
    [SerializeField] private List<Button> milestonesButton;
    [SerializeField] private List<TierButton> upgradesTierButton;
    [SerializeField] private List<MilestoneCost> costUpgrade;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject noneUpgrade;

    [Header("Hover")]
    [SerializeField] private HoverUI hoverUI;

    public void InitSelectMilestone()
    {
        milestones = database.AllMilestones;
        gameObject.SetActive(false);
        noneUpgrade.SetActive(true);

        selectButton.onClick.AddListener(SelectUpgrade);
        selectButton.interactable = false;

        InitMilestonesButton();
        InitUpgradeTierButton();

        DisableUpgradeTierButton();
        DisableUpgradeCost();
        DisableReward();
        ExitHoverMilesoneData();
    }
    public void OpenSelectionMilestone()
    {
        UpdateNewUnlock();
        gameObject.SetActive(true);

        if (currenMilestone != -1 && currentTierUpgrade != -1)
        {
            selectButton.interactable = !milestones[currenMilestone].tier[currentTierUpgrade].isBought;
        }
    }

    private void UpdateNewUnlock()
    {
        milestones = database.AllMilestones;

        for (int i = 0; i < milestonesButton.Count; i++)
        {
            milestonesButton[i].interactable = milestones[i].isUnlock;
        }
    }

    private void InitMilestonesButton()
    {
        for (int i = 0; i < milestonesButton.Count; i++)
        {
            int index = i;
            milestonesButton[index].onClick.AddListener(() => MilestoneClick(index));
            milestonesButton[index].interactable = milestones[index].isUnlock;
        }
    }

    private void InitUpgradeTierButton()
    {
        for (int i = 0; i < upgradesTierButton.Count; i++)
        {
            int index = i;
            upgradesTierButton[i].Button.onClick.AddListener(() => UpgradeTierClick(index));
        }
    }

    private void MilestoneClick(int index)
    {
        DisableUpgradeTierButton();

        currenMilestone = index;
        for (int i = 0; i < milestones[index].tier.Count; i++)
        {
            SOTier tier = milestones[index].tier[i];
            TierButton tierButton = upgradesTierButton[i];

            tierButton.Init(tier.icon, tier.nameTier);
            tierButton.Enable();
        }
    }

    private void DisableUpgradeTierButton()
    {
        for (int i = 0; i < upgradesTierButton.Count; i++)
        {
            upgradesTierButton[i].Disable();
        }
    }
    private void UpgradeTierClick(int index)
    {
        noneUpgrade.SetActive(false);
        selectButton.interactable = !milestones[currenMilestone].tier[index].isBought;
        currentTierUpgrade = index;
        InitOverviewTier(milestones[currenMilestone].tier[index]);
    }
    private void InitOverviewTier(SOTier tier)
    {
        DisableUpgradeCost();
        DisableReward();
        InitReward(tier);
        InitUpgradeCost(tier);
    }
    private void InitUpgradeCost(SOTier tier)
    {
        for (int i = 0; i < tier.tierCost.Count; i++)
        {
            TierCost cost = tier.tierCost[i];
            costUpgrade[i].HoverData = new HoverData(cost.item.Sprite, cost.item.NameItem, cost.valueCost);
            costUpgrade[i].Enable();
        }
    }
    private void InitReward(SOTier tier)
    {
        for (int i = 0; i < tier.rewards.buildingReward.Count; i++)
        {
            rewardTier[i].HoverData = new HoverData(tier.rewards.buildingReward[i].icon, tier.rewards.buildingReward[i].name);
            rewardTier[i].Enable();
        }
        int index = tier.rewards.buildingReward.Count;

        for (int i = 0; i < tier.rewards.recipeReward.Count; i++)
        {
            rewardTier[i + index].HoverData = new HoverData(tier.rewards.recipeReward[i].ItemOutput.Sprite, tier.rewards.recipeReward[i].NameRecipe);
            rewardTier[i + index].Enable();
        }
    }
    public void HoverMilestoneData(HoverData hoverData)
    {
        if (hoverData.haveValue)
        {
            hoverUI.UpdateUI(hoverData.icon, hoverData.name, hoverData.value);
        }
        else
        {
            hoverUI.UpdateUI(hoverData.icon, hoverData.name);
        }
    }
    public void ExitHoverMilesoneData() => hoverUI.Disable();
    private void DisableReward()
    {
        for (int i = 0; i < rewardTier.Count; i++)
        {
            rewardTier[i].Disable();
        }
    }
    private void DisableUpgradeCost()
    {
        for (int i = 0; i < costUpgrade.Count; i++)
        {
            costUpgrade[i].Disable();
        }
    }
    private void SelectUpgrade()
    {
        HUBCanvas.Instance.SetNewUpgrade(milestones[currenMilestone].tier[currentTierUpgrade], currenMilestone, currentTierUpgrade);

        PlayerUi.Instance.GetPingJalonTitle().text = milestones[currenMilestone].tier[currentTierUpgrade].nameTier;

        List<TierCost> listItemsCost = milestones[currenMilestone].tier[currentTierUpgrade].tierCost;

        for (int i = 0; i < listItemsCost.Count; i++)
        {
            PlayerUi.Instance.SetIsJalonActive(true);
            PlayerUi.Instance.GetPingJalonItems()[i].Enable();
            PlayerUi.Instance.GetPingJalonItems()[i].Init(listItemsCost[i].valueCost, Player.Instance.GetItemAmount(listItemsCost[i].item), listItemsCost[i].item);
        }
        for (int i = listItemsCost.Count; i < PlayerUi.Instance.GetPingJalonItems().Count; i++)
        {
            PlayerUi.Instance.GetPingJalonItems()[i].Disable();
        }
        HUBCanvas.Instance.OpenActiveMilestoneMenu();
    }
}