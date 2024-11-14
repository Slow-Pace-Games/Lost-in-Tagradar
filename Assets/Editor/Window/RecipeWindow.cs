using System.Linq;
using UnityEditor;
using UnityEngine;

public class RecipeWindow : EditorWindow
{
    private SODatabase database;
    private Vector2 scrollPosition;
    private string newItemName = "New Recipe";
    private MachineType machineType = MachineType.None;
    private Color select = new Color(1f, 0.5f, 0f, 1f);

    [MenuItem("Tools/Recipe Manager")]
    public static void ShowWindow()
    {
        RecipeWindow window = GetWindow<RecipeWindow>("Recipe Manager");

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

        EditorGUILayout.LabelField("Item Database", EditorStyles.boldLabel);

        DisplayHeader();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < database.AllRecipes.Count; i++)
        {
            DisplayRecipe(database.AllRecipes[i], i);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DisplayHeader()
    {
        GUILayout.BeginVertical();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save modifications"))
        {
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(database);
            for (int i = 0; i < database.AllRecipes.Count; i++)
            {
                EditorUtility.SetDirty(database.AllRecipes[i]);
            }
            AssetDatabase.SaveAssets();
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Create Recipe") && machineType != MachineType.None)
        {
            AddNewRecipe(newItemName);
        }

        GUI.backgroundColor = Color.white;
        GUILayout.BeginHorizontal(GUI.skin.box);
        newItemName = EditorGUILayout.TextField("Recipe Name", newItemName);

        for (int i = 1; i < (int)MachineType.Count; i++)
        {
            MachineType type = (MachineType)i;

            GUI.backgroundColor = machineType == type ? GUI.backgroundColor = select : Color.white;
            if (GUILayout.Button(type.ToString()))
            {
                machineType = type;
            }
        }

        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);
    }

    private void AddNewRecipe(string recipeName)
    {
        string path = "Assets/ScriptableObjects/Recipe/" + machineType.ToString() + "/" + recipeName + ".asset";
        if (AssetDatabase.LoadAssetAtPath<SOItems>(path) != null)
        {
            Debug.LogError("this item alredy exist");
            return;
        }

        SORecipe newRecipe = ScriptableObject.CreateInstance<SORecipe>();

        newRecipe.name = recipeName;
        newRecipe.NameRecipe = recipeName;
        database.AllRecipes.Add(newRecipe);
        newItemName = "New Recipe";
        newRecipe.machineRef = machineType;
        database.AllBuildingData.Where(building => building.machineType == machineType).Select(building => building).First().recipes.Add(newRecipe);
        AssetDatabase.CreateAsset(newRecipe, "Assets/ScriptableObjects/Recipe/" + machineType.ToString() + "/" + recipeName + ".asset");

        AssetDatabase.Refresh();
        EditorUtility.SetDirty(database);
        for (int i = 0; i < database.AllRecipes.Count; i++)
        {
            EditorUtility.SetDirty(database.AllRecipes[i]);
        }
        AssetDatabase.SaveAssets();
    }

    private void DisplayRecipe(SORecipe recipe, int index)
    {
        if (machineType != recipe.machineRef)
        {
            return;
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField(recipe.name, EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        recipe.NameRecipe = EditorGUILayout.TextField("Name", recipe.NameRecipe);

        GUI.color = select;
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        GUI.color = Color.white;

        recipe.ItemOutput = EditorGUILayout.ObjectField("Output", recipe.ItemOutput, typeof(SOItems), false) as SOItems;
        recipe.ValueStackOutput = EditorGUILayout.IntField("Value Output", recipe.ValueStackOutput);

        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = select;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = Color.white;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Input");
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add"))
        {
            AddNewInput(recipe);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        EditorGUI.indentLevel++;

        for (int i = 0; i < recipe.ItemsInput.Count; i++)
        {
            GUI.backgroundColor = select;
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUI.backgroundColor = Color.white;
            DisplayInput(recipe.ItemsInput[i]);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove"))
            {
                recipe.ItemsInput.RemoveAt(i);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        recipe.MeltTime = EditorGUILayout.FloatField("Melt Time", recipe.MeltTime);
        recipe.ElectricityCost = EditorGUILayout.FloatField("Electricity Cost", recipe.ElectricityCost);

        EditorGUILayout.LabelField("Description");
        recipe.Description = EditorGUILayout.TextArea(recipe.Description, GUILayout.Height(100f));

        EditorGUI.indentLevel--;

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Remove Recipe"))
        {
            EditorGUILayout.EndVertical();
            RemoveItem(index);
            GUI.FocusControl(null);
            return;
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndVertical();
    }
    
    private void AddNewInput(SORecipe recipe)
    {
        recipe.ItemsInput.Add(new SOInput());
    }

    private void DisplayInput(SOInput input)
    {
        input.Item = EditorGUILayout.ObjectField("Item", input.Item, typeof(SOItems), false) as SOItems;
        input.ValueStack = EditorGUILayout.IntField("Value input", input.ValueStack);
    }

    private void RemoveItem(int index)
    {
        SORecipe recipeToRemove = database.AllRecipes[index];
        database.AllRecipes.RemoveAt(index);
        SOBuildingData building = database.AllBuildingData.Where(building => building.recipes.Find(recipe => recipe == recipeToRemove)).Select(building => building).First();
        building.recipes.Remove(recipeToRemove);
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/Recipe/" + recipeToRemove.machineRef.ToString() + "/" + recipeToRemove.name + ".asset");
        AssetDatabase.SaveAssets();
    }
}