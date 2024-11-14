using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewDataBase", menuName = "DataBase", order = 3)]
public class SODatabase : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private List<SOItems> allItems;
    [SerializeField] private List<SORecipe> allRecipes;
    [SerializeField] private List<SOMilestone> allMilestones;
    [SerializeField] private List<SOBuildingData> allBuildingData;
    [SerializeField] private List<SORecipe> allRCRecipes;

    public List<SOItems> AllItems { get => allItems; }
    public List<SORecipe> AllRecipes { get => allRecipes; }
    public List<SOMilestone> AllMilestones { get => allMilestones; }
    public List<SOBuildingData> AllBuildingData { get => allBuildingData; }
    public List<SORecipe> AllRCRecipes { get => allRCRecipes; }

    public readonly DatabaseSaver initialData = new DatabaseSaver();

    #region Save
    public void ResetData() => initialData.ResetData(this);
    public void InitData() => initialData.SaveInitialData(this);
    public DatabaseSaver SaveData()
    {
        DatabaseSaver temp = new DatabaseSaver();
        temp.SaveInitialData(this);
        return temp;
    }
    #endregion

    #region Load
    public void LoadData(DatabaseSaver data)
    {
        if (data == null || data.itemDiscover == null || data.recipeDiscover == null || data.buildingDiscover == null || data.milestoneDatas == null)
        {
            return;
        }

        LoadItems(data.itemDiscover);
        LoadRecipe(data.recipeDiscover);
        LoadBuilding(data.buildingDiscover);
        LoadMilestone(data.milestoneDatas);
    }
    private void LoadItems(bool[] boolData)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            allItems[i].IsDiscover = boolData[i];
        }
    }
    private void LoadRecipe(bool[] boolData)
    {
        for (int i = 0; i < allRecipes.Count; i++)
        {
            allRecipes[i].isUnlock = boolData[i];
        }
    }
    private void LoadBuilding(bool[] boolData)
    {
        for (int i = 0; i < allBuildingData.Count; i++)
        {
            allBuildingData[i].isDiscovered = boolData[i];
        }
    }
    private void LoadMilestone(MilestoneData[] milestoneDatas)
    {
        for (int i = 0; i < allMilestones.Count; i++)
        {
            allMilestones[i].isUnlock = milestoneDatas[i].isUnlock;
            allMilestones[i].isFinished = milestoneDatas[i].isFinished;

            for (int j = 0; j < allMilestones[i].tier.Count; j++)
            {
                allMilestones[i].tier[j].isUnlock = milestoneDatas[i].tierDatas[j].isUnlock;
                allMilestones[i].tier[j].isBought = milestoneDatas[i].tierDatas[j].isFinished;
            }
        }
    }
    #endregion

    #region Debug
    public void InitId()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            allItems[i].id = i;
#if UNITY_EDITOR
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(allItems[i]);
            AssetDatabase.SaveAssets();
#endif
        }
    }
    #endregion

    #region Class
    public class DatabaseSaver
    {
        public bool[] itemDiscover;
        public bool[] recipeDiscover;
        public bool[] buildingDiscover;
        public MilestoneData[] milestoneDatas;

        #region Save
        public void SaveInitialData(SODatabase data)
        {
            itemDiscover = new bool[data.allItems.Count];
            recipeDiscover = new bool[data.AllRecipes.Count];
            buildingDiscover = new bool[data.allBuildingData.Count];
            milestoneDatas = new MilestoneData[data.allMilestones.Count];

            SaveBools(data.allRecipes);
            SaveBools(data.AllItems);
            SaveBools(data.AllBuildingData);
            SaveMilestone(data.allMilestones);
        }
        private void SaveBools(List<SOItems> dataItems)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                itemDiscover[i] = dataItems[i].IsDiscover;
            }
        }
        private void SaveBools(List<SORecipe> dataItems)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                recipeDiscover[i] = dataItems[i].isUnlock;
            }
        }
        private void SaveBools(List<SOBuildingData> dataItems)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                buildingDiscover[i] = dataItems[i].isDiscovered;
            }
        }
        private void SaveMilestone(List<SOMilestone> dataMilestone)
        {
            for (int i = 0; i < dataMilestone.Count; i++)
            {
                milestoneDatas[i] = new MilestoneData();
                milestoneDatas[i].tierDatas = new TierData[dataMilestone[i].tier.Count];

                milestoneDatas[i].isUnlock = dataMilestone[i].isUnlock;
                milestoneDatas[i].isFinished = dataMilestone[i].isFinished;

                for (int j = 0; j < dataMilestone[i].tier.Count; j++)
                {
                    milestoneDatas[i].tierDatas[j] = new TierData();
                    milestoneDatas[i].tierDatas[j].isUnlock = dataMilestone[i].tier[j].isUnlock;
                    milestoneDatas[i].tierDatas[j].isFinished = dataMilestone[i].tier[j].isBought;
                }
            }
        }
        #endregion

        #region Reset
        public void ResetData(SODatabase data)
        {
            ResetBools(data.allItems);
            ResetBools(data.allBuildingData);
            ResetBools(data.AllRecipes);
            ResetMilestoneDatas(data.AllMilestones);
        }

        private void ResetBools(List<SOItems> dataItems)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                dataItems[i].IsDiscover = itemDiscover[i];
            }
        }
        private void ResetBools(List<SORecipe> dataItems)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                dataItems[i].isUnlock = recipeDiscover[i];
            }
        }
        private void ResetBools(List<SOBuildingData> dataItems)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                dataItems[i].isDiscovered = buildingDiscover[i];
            }
        }
        private void ResetMilestoneDatas(List<SOMilestone> dataMilestone)
        {
            for (int i = 0; i < dataMilestone.Count; i++)
            {
                dataMilestone[i].isUnlock = milestoneDatas[i].isUnlock;
                dataMilestone[i].isFinished = milestoneDatas[i].isFinished;

                for (int j = 0; j < dataMilestone[i].tier.Count; j++)
                {
                    dataMilestone[i].tier[j].isUnlock = milestoneDatas[i].tierDatas[j].isUnlock;
                    dataMilestone[i].tier[j].isBought = milestoneDatas[i].tierDatas[j].isFinished;
                }
            }
        }
        #endregion
    }
    public class MilestoneData
    {
        public bool isUnlock;
        public bool isFinished;
        public TierData[] tierDatas;
    }
    public class TierData
    {
        public bool isUnlock;
        public bool isFinished;
    }
    #endregion
}