using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveMilestone : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject noneUpgrade;

    [Header("UI-Inventory")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private Transform hoverImage;
    [SerializeField] private Transform divider;
    [SerializeField] private Transform dragImage;

    [Header("Data")]
    [SerializeField] private SODatabase database;
    [SerializeField] private List<IconItemWithQuantity> inputs;
    [SerializeField] private SOTier currentUpgrade;
    [SerializeField] private int currentMilestone;
    [SerializeField] private int currentTierUpgrade;

    public void InitActiveMilestone()
    {
        upgradeButton.onClick.AddListener(PurshaseUpgrade);
        UpdateUI();
        gameObject.SetActive(false);
        upgradeButton.interactable = false;
    }

    public void OpenActiveMilestone()
    {
        gameObject.SetActive(true);
        upgradeButton.interactable = false;

        if (currentUpgrade != null)
        {
            UpdateValueInput();
            upgradeButton.interactable = CanPurshaseUpgrade();
        }
        CreateInventory();
    }
    private void CreateInventory()
    {
        Player.Instance.CreateInventory(inventoryPanel, hoverImage, divider, dragImage);
    }
    public void DestroyInventory()
    {
        Player.Instance.DestroyInventory();
    }
    public void ActiveNewUpgrade(SOTier upgrade, int milestoneIndex, int tierUpgradeIndex)
    {
        currentUpgrade = upgrade;
        currentMilestone = milestoneIndex;
        currentTierUpgrade = tierUpgradeIndex;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentUpgrade != null)
        {
            upgradeName.text = currentUpgrade.nameTier;
            upgradeButton.interactable = CanPurshaseUpgrade();
            noneUpgrade.SetActive(false);

            UpdateInput();
            return;
        }

        upgradeButton.interactable = false;
        upgradeName.text = "";
        noneUpgrade.SetActive(true);
    }

    private void UpdateInput()
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            inputs[i].Disable();
        }

        for (int i = 0; i < currentUpgrade.tierCost.Count; i++)
        {
            TierCost upgradeCost = currentUpgrade.tierCost[i];

            inputs[i].Init(upgradeCost.item.Sprite, upgradeCost.valueCost, Player.Instance.GetItemAmount(upgradeCost.item));
            inputs[i].Enable();
        }
    }

    private void UpdateValueInput()
    {
        for (int i = 0; i < currentUpgrade.tierCost.Count; i++)
        {
            TierCost upgradeCost = currentUpgrade.tierCost[i];

            inputs[i].UpdateQuantity(upgradeCost.valueCost, Player.Instance.GetItemAmount(upgradeCost.item));
        }
    }

    private bool CanPurshaseUpgrade()
    {
        for (int i = 0; i < currentUpgrade.tierCost.Count; i++)
        {
            TierCost upgradeCost = currentUpgrade.tierCost[i];
            if (Player.Instance.GetItemAmount(upgradeCost.item) < upgradeCost.valueCost)
            {
                return false;
            }
        }

        return true;
    }

    private void PurshaseUpgrade()
    {
        for (int i = 0; i < currentUpgrade.tierCost.Count; i++)
        {
            TierCost upgradeCost = currentUpgrade.tierCost[i];
            Player.Instance.RemoveItem(upgradeCost.item, upgradeCost.valueCost);
        }


        foreach (SOBuildingData building in currentUpgrade.rewards.buildingReward)
        {
            //TODO Player.Instance.UnlockBuilding(building.prefab);
            building.isDiscovered = true;
        }

        UpdateNewUnlock();

        currentUpgrade = null;
        currentMilestone = -1;
        currentTierUpgrade = -1;

        UpdateUI();
        PlayerUi.Instance.SetIsJalonActive(false);
        Loid.Instance.UpdateTuto(PlayerAction.PurshaseMilestone);
        HUBCanvas.Instance.OpenMilestoneMenu();
    }

    private bool IsAllTierBought()
    {
        for (int i = 0; i < database.AllMilestones[currentMilestone].tier.Count; i++)
        {
            if (!database.AllMilestones[currentMilestone].tier[i].isBought)
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateNewUnlock()
    {
        SOMilestone milestoneUpgraded = database.AllMilestones[currentMilestone];
        milestoneUpgraded.tier[currentTierUpgrade].isBought = true;

        foreach (SORecipe recipeReward in milestoneUpgraded.tier[currentTierUpgrade].rewards.recipeReward)
        {
            recipeReward.isUnlock = true;
            recipeReward.ItemOutput.IsDiscover = true;
        }

        HUBCanvas.Instance.UnlockNewRecipes(milestoneUpgraded.tier[currentTierUpgrade].rewards.recipeReward);

        if (IsAllTierBought())
        {
            milestoneUpgraded.isFinished = true;

            List<SOMilestone> milestonesPossibleUnlock = database.AllMilestones.Where(milestone => milestone.prerequisite.Find(prerequisite => prerequisite == milestoneUpgraded)).Select(milestone => milestone).ToList();

            for (int i = 0; i < milestonesPossibleUnlock.Count; i++)
            {
                if (ArePrerequisitesUnlocked(milestonesPossibleUnlock[i]))
                {
                    milestonesPossibleUnlock[i].isUnlock = true;
                }
            }
        }
    }

    private bool ArePrerequisitesUnlocked(SOMilestone milestone)
    {
        if (milestone.prerequisite.Count == 0)
        {
            return true;
        }

        foreach (SOMilestone prereq in milestone.prerequisite)
        {
            if (!prereq.isUnlock)
            {
                return false;
            }
        }

        return true;
    }
}