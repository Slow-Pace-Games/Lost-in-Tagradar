using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TierSelection : MonoBehaviour
{
    [SerializeField] private Transform milestoneContainer;
    [SerializeField] private Transform rewardContainer;
    [SerializeField] private SOMilestone milestones;
    [SerializeField] private Button prefabMilestone;
    [SerializeField] private Image prefabReward;

    private void Start()
    {
        InitMilestones();
    }

    public void InitMilestones()
    {
        for (int i = 0; i < milestoneContainer.childCount; ++i)
        {
            Destroy(milestoneContainer.GetChild(i).gameObject);
        }

        //for (int i = 0; i < milestones.SOTier.Count(); ++i)
        //{
        //    Button temp = Instantiate(prefabMilestone, milestoneContainer);
        //    temp.GetComponent<Tier>().tier = i;
        //}
    }

    public void DrawReward(int tier)
    {
        for (int i = 0; i < rewardContainer.childCount; ++i)
        {
            Destroy(rewardContainer.GetChild(i).gameObject);
        }

        //SOTier currTier = milestones.SOTier[tier];
        //int nbReward = currTier.BuildingUnlocked.Count() + currTier.BuildingUnlocked.Count();

        //for (int i = 0; i < currTier.BuildingUnlocked.Count(); ++i)
        //{
        //    Image temp = Instantiate(prefabReward, rewardContainer);
        //    temp.sprite = currTier.BuildingUnlocked[i].sprite;
        //}

        //for (int i = milestones.SOTier[tier].BuildingUnlocked.Count(); i < nbReward; ++i)
        //{

        //}
    }
}
