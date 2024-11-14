using System.Collections.Generic;
using UnityEngine;

//degager la class sans scriptable
//rajouter les prérequis sur la milestone

[CreateAssetMenu(fileName = "NewMilestone", menuName = "HUB/Milestone", order = 0)]
public class SOMilestone : ScriptableObject
{
    [Header("UI")]
    public string nameMilestone;

    [Header("Data")]
    public List<SOMilestone> prerequisite = new List<SOMilestone>();
    public List<SOTier> tier = new List<SOTier>();
    public bool isUnlock;
    public bool isFinished;
}