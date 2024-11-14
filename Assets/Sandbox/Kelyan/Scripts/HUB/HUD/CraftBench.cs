using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CraftBench : MonoBehaviour
{
    [Header("Craft")]
    [SerializeField] private SORecipe currentRecipe;
    [SerializeField] private Transform transformProdPanel;
    [SerializeField] private TextMeshProUGUI nameRecipe;
    [SerializeField] private Transform noneRecipeTransform;
    private float maxTimer;
    private float timer;

    [Header("   Ouput")]
    [SerializeField] private IconItemWithQuantity output;

    [Header("   Input")]
    [SerializeField] private List<IconItemWithQuantity> itemsInput;

    [Header("   Craft")]
    [SerializeField] private Button craftButton;
    [SerializeField] private Slider sliderProd;
    [SerializeField] private int nbClick;
    [SerializeField] private int nbClickNeed;

    [Header("UI-Inventory")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private Transform hoverImage;
    [SerializeField] private Transform divider;
    [SerializeField] private Transform dragImage;

    [Header("Recipes")]
    [SerializeField] private SODatabase database;
    private List<SORecipe> recipes;

    [Header("Prefab")]
    [SerializeField] private GameObject prefabButton;

    [Header("Container")]
    [SerializeField] private Transform containerButtonRecipe;

    public void InitCraftBenchRC()
    {
        recipes = database.AllRCRecipes;
        transformProdPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
        CreateCraftBenchRecipeButton();
        InitUICurrentRecipe();
    }

    public void InitCraftBench()
    {
        recipes = database.AllRecipes;
        transformProdPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
        CreateCraftBenchRecipeButton();
        InitUICurrentRecipe();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxTimer) 
        {
            timer = maxTimer;
        }

        if (PlayerInputManager.Instance.IsCraftKeyboardPressed())
        {
            Prod();
        }
    }

    private void CreateCraftBenchRecipeButton()
    {
        for (int i = 0; i < containerButtonRecipe.childCount; ++i)
        {
            Destroy(containerButtonRecipe.GetChild(i).gameObject);
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].isUnlock && recipes[i].ItemOutput != null)
            {
                GameObject newButton = Instantiate(prefabButton, containerButtonRecipe);
                newButton.GetComponent<ItemRecipeHUB>().Init(recipes[i].ItemOutput);

                SORecipe recipeSelected = recipes[i];
                newButton.GetComponent<Button>().onClick.AddListener(() => SetCurrentRecipe(recipeSelected));
            }
        }
    }

    private void SetCurrentRecipe(SORecipe recipeSelected)
    {
        currentRecipe = recipeSelected;

        nameRecipe.text = currentRecipe.NameRecipe;
        noneRecipeTransform.gameObject.SetActive(false);
        InitUICurrentRecipe();
    }
    private void InitUICurrentRecipe()
    {
        if (currentRecipe != null)
        {
            InitInputItemRecipe();
            output.Init(currentRecipe.ItemOutput.Sprite, currentRecipe.ValueStackOutput);
            InitCraftButton();
            transformProdPanel.gameObject.SetActive(true);
            return;
        }
        transformProdPanel.gameObject.SetActive(false);
    }
    private void InitInputItemRecipe()
    {
        for (int i = 0; i < itemsInput.Count; i++)
        {
            itemsInput[i].Disable();
        }

        for (int i = 0; i < currentRecipe.ItemsInput.Count; i++)
        {
            SOInput input = currentRecipe.ItemsInput[i];
            itemsInput[i].Init(input.Item.Sprite, input.ValueStack, Player.Instance.GetItemAmount(currentRecipe.ItemsInput[i].Item));
            itemsInput[i].Enable();
        }
    }
    private void InitCraftButton()
    {
        nbClick = 0;
        nbClickNeed = Mathf.CeilToInt(currentRecipe.MeltTime);

        sliderProd.value = 0;
        sliderProd.maxValue = nbClickNeed;

        craftButton.interactable = CanBeProd();
    }

    public void Prod()
    {
        if(timer >= maxTimer && CanBeProd())
        {
            timer = 0;
            nbClick++;
            sliderProd.value = nbClick;

            CheckProdClick();
        }       
    }

    private bool CanBeProd()
    {
        if (currentRecipe == null)
        {
            return false;
        }

        for (int i = 0; i < currentRecipe.ItemsInput.Count; i++)
        {
            if (currentRecipe.ItemsInput[i].ValueStack > Player.Instance.GetItemAmount(currentRecipe.ItemsInput[i].Item))
            {
                return false;
            }
        }

        if(!Player.Instance.CanAddItem(currentRecipe.ItemOutput))
        {
            return false;
        }

        return true;
    }
    private void CheckProdClick()
    {
        if (nbClick >= nbClickNeed)
        {
            UseRecipeInventory();
            UpdateUICurrentRecipe();
        }
    }
    private void UseRecipeInventory()
    {
        for (int i = 0; i < currentRecipe.ItemsInput.Count; i++)
        {
            SOInput input = currentRecipe.ItemsInput[i];
            Player.Instance.RemoveItem(input.Item, input.ValueStack);
        }

        if (currentRecipe.ItemOutput != null)
        {
            Player.Instance.AddItem(currentRecipe.ItemOutput, currentRecipe.ValueStackOutput);
        }
    }
    private void UpdateUICurrentRecipe()
    {
        for (int i = 0; i < currentRecipe.ItemsInput.Count; i++)
        {
            SOInput input = currentRecipe.ItemsInput[i];
            itemsInput[i].UpdateQuantity(input.ValueStack, Player.Instance.GetItemAmount(input.Item));
        }

        nbClick = 0;
        sliderProd.value = 0;

        craftButton.interactable = CanBeProd();
    }
    public void OpenCraftBench()
    {
        gameObject.SetActive(true);
        PlayerInputManager.Instance.EnableCrafting();
        timer = 0;
        maxTimer = 0.5f;
        if (currentRecipe != null)
        {
            UpdateUICurrentRecipe();
        }
        else
        {
            noneRecipeTransform.gameObject.SetActive(true);
        }
        CreateInventory();
    }

    public void OpenCraftBenchRC()
    {
        gameObject.SetActive(true);
        PlayerInputManager.Instance.EnableCrafting();
        timer = 0;
        maxTimer = 0.5f;
        if (currentRecipe != null)
        {
            UpdateUICurrentRecipe();
        }
        else
        {
            noneRecipeTransform.gameObject.SetActive(true);
        }
    }

    public void UnlockNewRecipes(List<SORecipe> recipesToUnlock)
    {
        for (int i = 0; i < recipesToUnlock.Count; i++)
        {
            GameObject newButton = Instantiate(prefabButton, containerButtonRecipe);
            SORecipe recipe = recipesToUnlock[i];
            newButton.GetComponent<ItemRecipeHUB>().Init(recipe.ItemOutput);
            newButton.GetComponent<Button>().onClick.AddListener(() => SetCurrentRecipe(recipe));
        }
    }

    private void CreateInventory()
    {
        Player.Instance.CreateInventory(inventoryPanel, hoverImage, divider, dragImage);
    }

    public void DestroyInventory()
    {
        Player.Instance.DestroyInventory();
    }
}