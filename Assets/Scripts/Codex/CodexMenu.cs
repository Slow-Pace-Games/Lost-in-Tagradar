using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using System;

public class CodexMenu : MonoBehaviour
{
    public GameObject middlePanel;

    [Header("Item Data")]
    [SerializeField] GameObject descriptionContainer;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;

    [Header("Recipe")]
    [SerializeField] GameObject recipesContainer;
    [SerializeField] GameObject recipesCostContainer;
    [SerializeField] GameObject objectCostPrefab;

    [Header("Production")]
    [SerializeField] GameObject produceContainer;

    #region General
    public void ActiveRecipeDescriptionContainer(bool isActive)
    {
        // Active or deactive Up Panel
        descriptionContainer.gameObject.SetActive(isActive);
    }

    public void ActiveRecipeContainer(bool isActive)
    {
        // Active or deactive recipe container (down panel)
        recipesContainer.gameObject.SetActive(isActive);
    }

    private void ChangeRecipeDescription(Sprite icon, string name, string description)
    {
        ActiveRecipeDescriptionContainer(true);
        // Change item description
        itemIcon.sprite = icon;
        itemName.text = name;
        itemDescription.text = description;
    }

    private void ChangeRecipeIngredient(SOItems item, int value, float productionRate = 0f)
    {
        // Add an element and how many needed
        GameObject newObjectCost = Instantiate(objectCostPrefab, recipesCostContainer.transform);
        ObjectCostContainer objectCostContainer = newObjectCost.GetComponent<ObjectCostContainer>();

        objectCostContainer.ingredientSprite.sprite = item.Sprite;
        objectCostContainer.ingredientAmount.text = value.ToString();
        objectCostContainer.ingredientName.text = item.NameItem;

        // Config button for switching to ingredient page in codex
        objectCostContainer.button.onClick.AddListener(() => Codex.Instance.CodexMenu.AddPartsToRecipes());
        objectCostContainer.button.onClick.AddListener(() => Codex.Instance.CodexMenu.ChangePartDescription(item));

        if (productionRate > 0f)
        {
            objectCostContainer.ingredientProduction.text = productionRate.ToString() + " per minute";
        }
        else
        {
            objectCostContainer.ingredientProduction.text = "";
        }
    }

    private void AddArrowAndIconAfterRecipe(Sprite ingredientIcon, int value, string ingredientName, float productionRate = 0f)
    {
        // Add arrow and Building icon after Items for recipe
        Instantiate(Codex.Instance.ArrowPrefab, recipesCostContainer.transform);
        GameObject objectImage = Instantiate(objectCostPrefab, recipesCostContainer.transform);
        ObjectCostContainer objectCostContainer = objectImage.GetComponent<ObjectCostContainer>();

        objectCostContainer.ingredientSprite.sprite = ingredientIcon;
        objectCostContainer.ingredientAmount.text = value.ToString();
        objectCostContainer.ingredientName.text = ingredientName;

        if (productionRate > 0f)
        {
            objectCostContainer.ingredientProduction.text = productionRate.ToString() + " per minute";
        }
        else
        {
            objectCostContainer.ingredientProduction.text = "";
        }
    }

    private void ChangeProducedValue(string machineName, float producePerMinute)
    {
        if (!produceContainer.activeSelf)
        {
            produceContainer.SetActive(true);
        }
        produceContainer.transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = machineName;
        produceContainer.transform.GetChild(1).transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = producePerMinute.ToString() + " per minute";
    }
    #endregion

    #region Parts and Equipment
    public void AddPartsToRecipes()
    {
        Codex.Instance.DestroyContent(middlePanel.transform.GetChild(0).transform);
        SODatabase database = Codex.Instance.Database;
        float height = 30f;

        for (int i = 0; i < database.AllItems.Count; i++)
        {
            if (!database.AllItems[i].isEquipable)
            {
                if (database.AllItems[i].IsDiscover)
                {
                    GameObject newInBox = Instantiate(Codex.Instance.ObjectTitlePrefab, middlePanel.transform.GetChild(0).transform);
                    newInBox.transform.GetChild(0).GetComponent<Image>().sprite = Codex.Instance.Database.AllItems[i].Sprite;
                    newInBox.GetComponentInChildren<TextMeshProUGUI>().text = Codex.Instance.Database.AllItems[i].NameItem;
                    int index = i;
                    newInBox.GetComponentInChildren<Button>().onClick.AddListener(() => ChangePartDescription(Codex.Instance.Database.AllItems[index]));
                    height += 50f;
                }
            }
        }

        RectTransform rect = middlePanel.transform.GetChild(0).GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, height);
    }
    public void AddEquipmentToRecipes()
    {
        Codex.Instance.DestroyContent(middlePanel.transform.GetChild(0).transform);
        SODatabase database = Codex.Instance.Database;
        float height = 30f;

        for (int i = 0; i < database.AllItems.Count; i++)
        {
            if (database.AllItems[i].isEquipable)
            {
                if (database.AllItems[i].IsDiscover)
                {
                    GameObject newInBox = Instantiate(Codex.Instance.ObjectTitlePrefab, middlePanel.transform.GetChild(0).transform);
                    newInBox.transform.GetChild(0).GetComponent<Image>().sprite = Codex.Instance.Database.AllItems[i].Sprite;
                    newInBox.GetComponentInChildren<TextMeshProUGUI>().text = Codex.Instance.Database.AllItems[i].NameItem;
                    int index = i;
                    newInBox.GetComponentInChildren<Button>().onClick.AddListener(() => ChangePartDescription(Codex.Instance.Database.AllItems[index]));
                    height += 50f;
                }
            }
        }

        RectTransform rect = middlePanel.transform.GetChild(0).GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, height);
    }
    public void ChangePartDescription(SOItems item)
    {
        ChangeRecipeDescription(item.Sprite, item.NameItem, item.description);

        // Reset Down panel
        Codex.Instance.DestroyContent(recipesCostContainer.transform);

        // Search recipe in database
        List<SORecipe> recipe = Codex.Instance.Database.AllRecipes.Where(recipe => recipe.ItemOutput == item).ToList();

        if (recipe.Count > 0)
        {
            // Add production infos
            ActiveRecipeContainer(true);
            float producePerMinute = (60f / recipe[0].MeltTime) * recipe[0].ValueStackOutput;

            // Add Items for recipe if recipe exist
            for (int i = 0; i < recipe[0].ItemsInput.Count; i++)
            {
                SOInput soInput = recipe[0].ItemsInput[i];
                float neededAmount = producePerMinute * soInput.ValueStack;
                ChangeRecipeIngredient(soInput.Item, soInput.ValueStack, neededAmount);
            }
            AddArrowAndIconAfterRecipe(item.Sprite, recipe[0].ValueStackOutput, item.NameItem, producePerMinute);


            ChangeProducedValue(recipe[0].machineRef.ToString(), producePerMinute);
        }
        else
        {
            ActiveRecipeContainer(false);
        }
    }
    #endregion

    #region Buildings
    public void AddBuildingsToRecipes()
    {
        Codex.Instance.DestroyContent(middlePanel.transform.GetChild(0).transform);
        float height = 30f;
        SODatabase database = Codex.Instance.Database;

        for (int i = 0; i < database.AllBuildingData.Count; i++)
        {
            if (database.AllBuildingData[i].isDiscovered)
            {
                GameObject newInBox = Instantiate(Codex.Instance.ObjectTitlePrefab, middlePanel.transform.GetChild(0).transform);
                newInBox.transform.GetChild(0).GetComponent<Image>().sprite = Codex.Instance.Database.AllBuildingData[i].icon;
                newInBox.GetComponentInChildren<TextMeshProUGUI>().text = Codex.Instance.Database.AllBuildingData[i].name;
                int index = i;
                newInBox.GetComponentInChildren<Button>().onClick.AddListener(() => ChangeBuildingDescription(Codex.Instance.Database.AllBuildingData[index]));
                height += 50f;
            }
        }

        RectTransform rect = middlePanel.transform.GetChild(0).GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, height);
    }

    private void ChangeBuildingDescription(SOBuildingData building)
    {
        ChangeRecipeDescription(building.icon, building.name, building.description);

        // Reset Down panel
        Codex.Instance.DestroyContent(recipesCostContainer.transform);
        ActiveRecipeContainer(true);


        // Add Items for recipe
        for (int i = 0; i < building.costs.Count; i++)
        {
            BuildingCost buildingCost = building.costs[i];
            ChangeRecipeIngredient(buildingCost.item, buildingCost.value);
        }
        AddArrowAndIconAfterRecipe(building.icon, 1, building.name);

        produceContainer.SetActive(false);
    }
    #endregion
}
