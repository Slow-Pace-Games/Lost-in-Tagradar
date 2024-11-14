using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeTier", menuName = "HUB/UpgradeTier", order = 1)]
public class SOTier : ScriptableObject
{
    [Header("Data")]
    public bool isUnlock;
    public bool isBought;
    public string nameTier;

    [Header("Sprite")]
    public Sprite icon;
    public Sprite iconBought;

    [Header("Cost")]
    public List<TierCost> tierCost = new List<TierCost>();

    [Header("Reward")]
    public RewardTier rewards = new RewardTier();
}

[System.Serializable]
public class RewardTier
{
    [Header("Reward")]
    public List<SORecipe> recipeReward = new List<SORecipe>();
    public List<SOBuildingData> buildingReward = new List<SOBuildingData>();
}

[System.Serializable]
public class TierCost
{
    [Header("Cost")]
    public SOItems item;
    public int valueCost;
}