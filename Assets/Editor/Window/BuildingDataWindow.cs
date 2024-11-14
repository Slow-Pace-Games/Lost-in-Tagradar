using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BuildingDataWindow : EditorWindow
{
    private SODatabase database;
    private Vector2 scrollPosition;
    private string newBuildingName = "New Building";

    [MenuItem("Tools/Building Manager")]
    public static void ShowWindow()
    {
        BuildingDataWindow window = GetWindow<BuildingDataWindow>("Building Manager");

        SODatabase database = Resources.Load<SODatabase>("DataBase");
        window.SetDatabase(database);
    }

    private void SetDatabase(SODatabase database)
    {
        this.database = database;
    }

    private void OnGUI()
    {
        if (database == null)
        {
            EditorGUILayout.LabelField("No database loaded in resources folder");
            return;
        }

        DisplayHeader();


        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < database.AllBuildingData.Count; i++)
        {
            DisplayBuilding(database.AllBuildingData[i], i);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DisplayHeader()
    {
        GUILayout.BeginVertical();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save modifications"))
        {
            Save();
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Create Building"))
        {
            AddNewBuilding(newBuildingName);
        }

        GUI.backgroundColor = Color.white;
        GUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void AddNewBuilding(string buildingName)
    {
        string path = "Assets/ScriptableObjects/BuildingData/" + buildingName + ".asset";
        if (AssetDatabase.LoadAssetAtPath<SOBuildingData>(path) != null)
        {
            Debug.LogError("this building alredy exist");
            return;
        }

        SOBuildingData newBuilding = ScriptableObject.CreateInstance<SOBuildingData>();
        database.AllBuildingData.Add(newBuilding);
        AssetDatabase.CreateAsset(newBuilding, path);
        newBuildingName = "New building";

        Save();
    }

    private void DisplayBuilding(SOBuildingData building, int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField(building.name, EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        DisplayRanks(building.ranks);
        GUILayout.Space(5);
        DisplayCosts(building.costs);
        GUILayout.Space(5);
        DisplayRecipes(building);
        GUILayout.Space(5);
        DisplaySingleValue(building);
        EditorGUI.indentLevel--;

        RemoveBuildingButton(index);
        EditorGUILayout.EndVertical();
    }

    #region Ranks
    private void DisplayRanks(List<SOMachineRank> ranks)
    {
        HeaderRanks(ranks);
        GUILayout.Space(5);
        EditorGUI.indentLevel++;

        for (int i = 0; i < ranks.Count; i++)
        {
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("Rank " + i.ToString(), EditorStyles.boldLabel, GUILayout.Width(75f));

            DisplayRank(ranks[i]);
            RemoveRank(ranks, i);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        EditorGUI.indentLevel--;
    }
    private void HeaderRanks(List<SOMachineRank> ranks)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ranks : ", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("+", GUILayout.Width(50f)))
        {
            ranks.Add(new SOMachineRank());
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }
    private void DisplayRank(SOMachineRank rank)
    {
        rank.percentTimer = EditorGUILayout.Slider("Timer", rank.percentTimer, 0f, 100f);
        rank.percentEnergy = EditorGUILayout.Slider("Energy", rank.percentEnergy, 0f, 100f);
        rank.texture = EditorGUILayout.ObjectField("Texture", rank.texture, typeof(Material), false) as Material;
    }
    private void RemoveRank(List<SOMachineRank> ranks, int index)
    {
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(50f)))
        {
            ranks.RemoveAt(index);
            Save();
        }
        GUI.backgroundColor = Color.white;
    }
    #endregion

    #region Costs
    private void DisplayCosts(List<BuildingCost> costs)
    {
        HeaderCosts(costs);
        GUILayout.Space(5);
        EditorGUI.indentLevel++;

        for (int i = 0; i < costs.Count; i++)
        {
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("Cost " + i.ToString(), EditorStyles.boldLabel, GUILayout.Width(75f));

            DisplayCost(costs[i]);
            RemoveCost(costs, i);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        EditorGUI.indentLevel--;
    }
    private void HeaderCosts(List<BuildingCost> costs)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Costs : ", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("+", GUILayout.Width(50f)))
        {
            costs.Add(new BuildingCost());
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }
    private void DisplayCost(BuildingCost cost)
    {
        cost.item = EditorGUILayout.ObjectField("Item", cost.item, typeof(SOItems), false) as SOItems;
        cost.value = EditorGUILayout.IntField("Value", cost.value);
    }
    private void RemoveCost(List<BuildingCost> costs, int index)
    {
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(50f)))
        {
            costs.RemoveAt(index);
            Save();
        }
        GUI.backgroundColor = Color.white;
    }
    #endregion

    #region Recipes
    private void DisplayRecipes(SOBuildingData building)
    {
        HeaderRecipes(building);
        GUILayout.Space(5);
        EditorGUI.indentLevel++;

        if (building.hasRecipe)
        {
            for (int i = 0; i < building.recipes.Count; i++)
            {
                GUI.backgroundColor = Color.black;
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.LabelField("Recipe " + i.ToString(), EditorStyles.boldLabel, GUILayout.Width(100f));

                building.recipes[i] = EditorGUILayout.ObjectField(building.recipes[i], typeof(SORecipe), false) as SORecipe;
                RemoveRecipe(building.recipes, i);

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
        }
        else
        {
            building.recipes.Clear();
        }

        EditorGUI.indentLevel--;
    }
    private void HeaderRecipes(SOBuildingData building)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Recipes : ", EditorStyles.boldLabel);
        building.hasRecipe = EditorGUILayout.Toggle("Has Recipes : ", building.hasRecipe);
        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("+", GUILayout.Width(50f)))
        {
            building.recipes.Add(null);
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

    }
    private void RemoveRecipe(List<SORecipe> recipes, int index)
    {
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(50f)))
        {
            recipes.RemoveAt(index);
            Save();
        }
        GUI.backgroundColor = Color.white;
    }
    #endregion

    #region Other
    private void DisplaySingleValue(SOBuildingData building)
    {
        EditorGUILayout.LabelField("Data :", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        GUI.backgroundColor = Color.black;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = Color.white;

        building.prefab = EditorGUILayout.ObjectField("Prefab", building.prefab, typeof(GameObject), false) as GameObject;
        building.icon = EditorGUILayout.ObjectField("Icon", building.icon, typeof(Sprite), false) as Sprite;
        EditorGUILayout.LabelField("Description");
        building.description = EditorGUILayout.TextArea(building.description,GUILayout.Height(100f));

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
    #endregion

    private void RemoveBuildingButton(int index)
    {
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Remove Building"))
        {
            RemoveBuilding(index);

            EditorGUILayout.EndVertical();
            GUI.FocusControl(null);
            return;
        }
        GUI.backgroundColor = Color.white;
    }

    private void RemoveBuilding(int index)
    {
        SOBuildingData itemToRemove = database.AllBuildingData[index];
        database.AllBuildingData.RemoveAt(index);

        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/BuildingData/" + itemToRemove.name + ".asset");
        AssetDatabase.SaveAssets();
        Save();
    }

    private void Save()
    {
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(database);
        for (int i = 0; i < database.AllBuildingData.Count; i++)
        {
            EditorUtility.SetDirty(database.AllBuildingData[i]);
        }
        AssetDatabase.SaveAssets();
    }
}