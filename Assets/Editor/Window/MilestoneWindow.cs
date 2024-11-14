using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilestoneWindow : EditorWindow
{
    private SODatabase database;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Milestone Manager")]
    public static void ShowWindow()
    {
        MilestoneWindow window = GetWindow<MilestoneWindow>("Milestone Manager");

        SODatabase database = Resources.Load<SODatabase>("DataBase");
        window.SetDatabase(database);
    }

    private void SetDatabase(SODatabase database)
    {
        this.database = database;
    }
    private void NoDatabase()
    {
        if (database == null)
        {
            EditorGUILayout.LabelField("No database loaded in resources folder");
            return;
        }
    }

    private void OnGUI()
    {
        NoDatabase();
        Header();
        Body();
    }

    private void Header()
    {
        GUILayout.BeginVertical();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save modifications"))
        {
            Save();
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Create Milestone"))
        {
            AddNewMilestone();
        }

        GUI.backgroundColor = Color.white;
        GUILayout.EndVertical();
        GUILayout.Space(5);
    }
    private void Body()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < database.AllMilestones.Count; i++)
        {
            DisplayMilestone(database.AllMilestones[i], i);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DisplayMilestone(SOMilestone milestone, int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        HeaderMilestone(milestone);

        EditorGUI.indentLevel++;
        MilestonePrerequisite(milestone.prerequisite);
        GUILayout.Space(5);
        MilestoneTier(milestone.tier, index.ToString());
        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();
    }
    private void HeaderMilestone(SOMilestone milestone)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(milestone.nameMilestone, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.black;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = Color.white;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        milestone.isUnlock = EditorGUILayout.Toggle("Is Unlock", milestone.isUnlock);
        milestone.isFinished = EditorGUILayout.Toggle("Is Finished", milestone.isFinished);

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    #region Prerequisite
    private void MilestonePrerequisite(List<SOMilestone> prerequisites)
    {
        HeaderPrerequisite(prerequisites);
        BodyPrerequisites(prerequisites);
    }
    private void HeaderPrerequisite(List<SOMilestone> prerequisites)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prerequisites :", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("+", GUILayout.Width(50f)))
        {
            AddPrerequisite(prerequisites);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }
    private void BodyPrerequisites(List<SOMilestone> prerequisites)
    {
        EditorGUI.indentLevel++;
        for (int i = 0; i < prerequisites.Count; i++)
        {
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("Prerequisite " + i.ToString(), GUILayout.MaxWidth(125f));
            prerequisites[i] = EditorGUILayout.ObjectField(prerequisites[i], typeof(SOMilestone), false) as SOMilestone;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.MaxWidth(50f)))
            {
                prerequisites.RemoveAt(i);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
    }
    private void AddPrerequisite(List<SOMilestone> prerequisites)
    {
        prerequisites.Add(null);
    }
    #endregion

    #region Tier
    private void MilestoneTier(List<SOTier> tiers, string milestoneIndex)
    {
        HeaderTier(tiers, milestoneIndex);
        BodyTier(tiers, milestoneIndex);
    }
    private void HeaderTier(List<SOTier> tiers, string milestoneIndex)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Tiers :", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("+", GUILayout.MaxWidth(50f)))
        {
            AddTier(tiers, milestoneIndex);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
    }
    private void BodyTier(List<SOTier> tiers, string milestoneIndex)
    {
        EditorGUI.indentLevel++;
        for (int i = 0; i < tiers.Count; i++)
        {
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUI.backgroundColor = Color.white;

            DisplayTier(tiers[i], tiers, milestoneIndex, i);

            EditorGUILayout.EndVertical();
        }
        EditorGUI.indentLevel--;
    }
    private void DisplayTier(SOTier tier, List<SOTier> tiers, string milestoneIndex, int index)
    {
        HeaderTier(tier, tiers, milestoneIndex, index);
        BodyTier(tier);
    }
    private void HeaderTier(SOTier tier, List<SOTier> tiers, string milestoneIndex, int index)
    {
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("X", GUILayout.MaxWidth(50f)))
        {
            tiers[index] = null;
            RenameMilestoneTier(milestoneIndex, tiers);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.BeginVertical();
        tier.nameTier = EditorGUILayout.TextField("Name", tier.nameTier, GUILayout.MaxWidth(300f));
        tier.isUnlock = EditorGUILayout.Toggle("Is Unlock", tier.isUnlock, GUILayout.MaxWidth(150f));
        tier.isBought = EditorGUILayout.Toggle("Is Bought", tier.isBought, GUILayout.MaxWidth(150f));
        EditorGUILayout.EndVertical();
    }
    private void BodyTier(SOTier tier)
    {
        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        tier.icon = EditorGUILayout.ObjectField("Icon", tier.icon, typeof(Sprite), false) as Sprite;
        tier.iconBought = EditorGUILayout.ObjectField("Icon Bought", tier.iconBought, typeof(Sprite), false) as Sprite;
        EditorGUILayout.EndHorizontal();

        TierCosts(tier.tierCost);
        TierReward(tier.rewards);
    }
    private void TierCosts(List<TierCost> tierCosts)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Costs : ", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("+", GUILayout.MaxWidth(50f)))
        {
            tierCosts.Add(new TierCost());
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        for (int i = 0; i < tierCosts.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            tierCosts[i].item = EditorGUILayout.ObjectField("Item", tierCosts[i].item, typeof(SOItems), false) as SOItems;
            tierCosts[i].valueCost = EditorGUILayout.IntField("Value", tierCosts[i].valueCost);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.MaxWidth(50f)))
            {
                tierCosts.RemoveAt(i);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        GUILayout.Space(5f);
    }
    private void TierReward(RewardTier rewards)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rewards : ", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField("Recipe Rewards : ", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("+", GUILayout.MaxWidth(50f)))
        {
            rewards.recipeReward.Add(null);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;

        for (int i = 0; i < rewards.recipeReward.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            rewards.recipeReward[i] = EditorGUILayout.ObjectField(rewards.recipeReward[i], typeof(SORecipe), false) as SORecipe;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.MaxWidth(50f)))
            {
                rewards.recipeReward.RemoveAt(i);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Building Rewards : ", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("+", GUILayout.MaxWidth(50f)))
        {
            rewards.buildingReward.Add(null);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;

        for (int i = 0; i < rewards.buildingReward.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            rewards.buildingReward[i] = EditorGUILayout.ObjectField(rewards.buildingReward[i], typeof(SOBuildingData), false) as SOBuildingData;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.MaxWidth(50f)))
            {
                rewards.buildingReward.RemoveAt(i);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
    private void AddTier(List<SOTier> tiers, string milestoneIndex)
    {
        SOTier newTier = ScriptableObject.CreateInstance<SOTier>();

        AssetDatabase.CreateAsset(newTier, "Assets/ScriptableObjects/Milestones/Milestone" + milestoneIndex + "/M" + milestoneIndex + "Tier" + tiers.Count.ToString() + ".asset");
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        tiers.Add(newTier);
    }
    #endregion

    private void RenameMilestoneTier(string milestoneIndex, List<SOTier> tiers)
    {
        string path = "Assets/ScriptableObjects/Milestones/Milestone" + milestoneIndex + "/M" + milestoneIndex + "Tier";
        string endPath = ".asset";
        string newName;
        int index = 0;

        for (int i = 0; i < tiers.Count; i++)
        {
            if (tiers[i] != null)
            {
                newName = "M" + milestoneIndex + "Tier" + i.ToString();
                AssetDatabase.RenameAsset(path + index.ToString() + endPath, newName);
            }
            else
            {
                tiers.RemoveAt(i);
                AssetDatabase.DeleteAsset(path + i.ToString() + endPath);
                i--;
            }
            index++;
        }
    }
    private void AddNewMilestone()
    {
        SOMilestone newMilestone = ScriptableObject.CreateInstance<SOMilestone>();
        newMilestone.nameMilestone = "Milestone " + database.AllMilestones.Count.ToString();

        AssetDatabase.CreateFolder("Assets/ScriptableObjects/Milestones", "Milestone" + database.AllMilestones.Count.ToString());
        AssetDatabase.CreateAsset(newMilestone, "Assets/ScriptableObjects/Milestones/Milestone" + database.AllMilestones.Count.ToString() + "/Milestone" + database.AllMilestones.Count.ToString() + ".asset");
        AssetDatabase.SaveAssets();

        database.AllMilestones.Add(newMilestone);
    }
    private void Save()
    {
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(database);
        for (int i = 0; i < database.AllMilestones.Count; i++)
        {
            EditorUtility.SetDirty(database.AllMilestones[i]);
            for (int j = 0; j < database.AllMilestones[i].tier.Count; j++)
            {
                EditorUtility.SetDirty(database.AllMilestones[i].tier[j]);
            }
        }
        AssetDatabase.SaveAssets();
    }
}