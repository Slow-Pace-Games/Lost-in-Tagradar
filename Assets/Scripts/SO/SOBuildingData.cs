using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Building Data", order = 4)]
public class SOBuildingData : ScriptableObject
{
    [Header("Data")]
    public List<SOMachineRank> ranks = new List<SOMachineRank>();
    public List<BuildingCost> costs = new List<BuildingCost>();
    public List<SORecipe> recipes = new List<SORecipe>();
    public MachineType machineType = MachineType.None;
    public MachineClass machineClass = MachineClass.Special;
    public PrefabsType prefabType = PrefabsType.Building;
    public bool isDiscovered = false;

    [Header("Visuel")]
    public GameObject prefab;
    public Sprite icon;
    public bool hasRecipe = true;
    public string description;

    [Header("Audio")]
    public SOAudioContainer sounds;
    public AudioClip discoverySound;
}

[System.Serializable]
public class SOMachineRank
{
    [Header("Rank Data")]
    public float percentTimer;
    public float percentEnergy;
    public Material texture;
}

[System.Serializable]
public class BuildingCost
{
    [Header("Cost")]
    public SOItems item;
    public int value;
}

public enum MachineRank
{
    Tier1,
    Tier2,
    Tier3,
    Tier4,
}

public enum MachineType
{
    None,
    Furnace,
    Constructor,
    Assembler,
    Foundry,
    Count,
}

public enum MachineClass
{
    Special,
    Production,
    Power,
    Logistics,
    Organization,
    Count
}