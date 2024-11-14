using System.Collections.Generic;
using UnityEngine;

public class DiscoveryManager : MonoBehaviour
{
    [SerializeField] SODatabase database;

    [Tooltip("Items discovered on start")]
    [SerializeField] List<SOItems> discoveredItems;
    [Tooltip("Buildings discovered on start")]
    [SerializeField] List<SOBuildingData> discoveredBuildings;
    [Tooltip("Recipes unlock on start")]
    [SerializeField] List<SORecipe> unlockRecipes;

    // Start is called before the first frame update
    void Start()
    {
        InitForNewGame();
    }

    private void InitForNewGame()
    {
        // Reset all element in database to undiscovered
        foreach (SOItems item in database.AllItems)
        {
            item.IsDiscover = false;
        }

        foreach (SOBuildingData building in database.AllBuildingData)
        {
            building.isDiscovered = false;
        }

        foreach (SORecipe recipe in database.AllRecipes)
        {
            recipe.isUnlock = false;
        }

        foreach (SOMilestone milestone in database.AllMilestones)
        {
            milestone.isUnlock = false;
            milestone.isFinished = false;
            foreach (SOTier tier in milestone.tier)
            {
                tier.isBought = false;
            }
        }

        //// Active Items, Buildings and Recipes unlock on start
        //for (int i = 0; i < discoveredItems.Count; i++)
        //{
        //    discoveredItems[i].IsDiscover = true;
        //}

        for (int i = 0; i < discoveredBuildings.Count; i++)
        {
            discoveredBuildings[i].isDiscovered = true;
        }

        for (int i = 0; i < unlockRecipes.Count; i++)
        {
            unlockRecipes[i].isUnlock = true;
        }

        // Set Milestone 0 to discovered on start
        database.AllMilestones[0].isUnlock = true;
    }
}
