using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//je pense que y'a trop de delegate, je pense qui va falloir que tu donne une ref sur la machine mais pas le code directement plutôt sur interface de laquelle tu pourras appellé les fonctions qu'il te faut
//et bien pensé a la delet quand tu quitte l'interface sa te permettera de mieux gérer

//il faudrait aussi que lorsque l'interface est fermé (active a false) il faudrait que le canvas en entier soit mis a false pour enlever les raycast qui fait des rayons en continue c une opti tkt

//tu peux aussi prévoir le hud dans le sens ou certain truc ne sont même pas a instantié quand tu ouvre ou au start du code on sait par exemple que le nombre d'input sera max = 2 et ouput = 1
//donc tu peux les mettre toi même dans unity puis les référencé ou les créer au start et gardé la référence sur les object que ta créer (en gardant le code évidemment) et juste changer les sprite donné etc
//et les enebale disable selon les cas pareil pour les recipe tu peux en créer 10 au début -je crois même pas qu'on aura de machine avec autant de recipe mais on prévois- et les réutiliser a chaque fois 
//sachant que les layout font l'order en fonction de ceux activé donc il ne prend pas en compte ceux désac en sans fous donc pour les order

public class MachineHUDManager : MonoBehaviour
{
    #region Singleton
    private static MachineHUDManager instance;
    public static MachineHUDManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
    #endregion

    [Header("Managers")]
    [SerializeField] private SelectRecipeManager recipeMenu;
    [SerializeField] private ProductionHUDManager productionMenu;
    [SerializeField] private DrillHUDManager drillMenu;
    [SerializeField] private GeneratorHUDManager generatorMenu;

    [Header("Production Menu Inventory")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private Transform hoverImage;
    [SerializeField] private Transform divider;
    [SerializeField] private Transform dragImage;
    
    [Header("Drill Menu Inventory")]
    [SerializeField] private Transform inventoryPanelDrill;
    [SerializeField] private Transform hoverImageDrill;
    [SerializeField] private Transform dividerDrill;
    [SerializeField] private Transform dragImageDrill; 
    
    [Header("Generator Menu Inventory")]
    [SerializeField] private Transform inventoryPanelGenerator;
    [SerializeField] private Transform hoverImageGenerator;
    [SerializeField] private Transform dividerGenerator;
    [SerializeField] private Transform dragImageGenerator;

    //Delegates
    public delegate void OnHover(int index);
    public OnHover onHover;

    public delegate void OnClickRecipe(SORecipe currentRecipe);
    public OnClickRecipe onClickRecipe;

    public delegate void OnInputStackChange(List<int> inputStackList);
    public OnInputStackChange onInputStackChange;

    public delegate void OnOutputStackChange(List<int> outputStackList);
    public OnOutputStackChange onOutputStackChange;

    public delegate void OnMachineStateChange(bool isProducting);
    public OnMachineStateChange onMachineStateChange;

    public delegate void OnMaxTimerChange(float maxTimer);
    public OnMaxTimerChange onMaxTimerChange;

    public delegate void OnStackValueChange(int nbStackToAdd, bool isInInput, int index, SOItems itemType);
    public OnStackValueChange onStackValueChange;

    public delegate void OnTransitionChange(SOItems transitionCochon);
    public OnTransitionChange onTransitionChange; 
    
    public delegate void OnIsAlimentedChange(bool isAlimented);
    public OnIsAlimentedChange onIsAlimentedChange;

    public delegate void OnUpdateUIGenerator(SOTransition currentTransition);
    public OnUpdateUIGenerator onUpdateUIGenerator;

    private float currentTimer;
    private bool isAlimented;

    public bool isOpen = false;

    private void Start()
    {
        onClickRecipe += ClickRecipe;
    }

    private void ClickRecipe(SORecipe currentRecipe)
    {
        productionMenu.UpdateUIMachine(currentRecipe, currentTimer, isAlimented);
        ActiveProductionMenu();
    }

    public void InputStackChange(List<int> inputStackList)
    {
        if (isOpen && onInputStackChange != null)
        {
            onInputStackChange?.Invoke(inputStackList);
        }
    }

    public void OutputStackChange(List<int> outputStackList)
    {
        if (isOpen && onOutputStackChange != null)
        {
            onOutputStackChange?.Invoke(outputStackList);
        }
    }

   
    public void IsMachineProducingChange(bool isMachineProducing)
    {
        if (isOpen && onMachineStateChange != null)
        {
            onMachineStateChange?.Invoke(isMachineProducing);
        }
    }
    
    public void IsMachineAlimentedChange(bool isAlimented)
    {
        if (isOpen && onIsAlimentedChange != null)
        {
            onIsAlimentedChange?.Invoke(isAlimented);
        }
    }

    public void MaxTimerChange(float maxTimer)
    {
        if (isOpen && onMaxTimerChange != null)
        {
            onMaxTimerChange?.Invoke(maxTimer);
        }
    }

    public void OpenMachineMenu(List<SORecipe> recipes, SORecipe currentRecipe, float currentTimer, bool isAlimented)
    {
        isOpen = true;

        this.isAlimented = isAlimented;
        if (currentRecipe == null)
        {
            ActiveRecipeMenu();
        }
        else
        {
            this.currentTimer = currentTimer;
            ActiveProductionMenu();
            productionMenu.InitProductionMenu(currentRecipe, currentTimer, isAlimented);
        }

        recipeMenu.InitRecipesMenu(recipes);
        Player.Instance.CreateInventory(inventoryPanel, hoverImage, divider, dragImage);
    }
    public void OpenDrillMenu(SOTransition currentRecipe, float currentTimer, float maxTimer, bool isProducing)
    {
        isOpen = true;
        this.currentTimer = currentTimer;
        ActiveDrillMenu();
        drillMenu.InitDrillMenu(currentRecipe, currentTimer, maxTimer, isProducing);
        Player.Instance.CreateInventory(inventoryPanelDrill, hoverImageDrill, dividerDrill, dragImageDrill);
    }

    public void OpenGeneratorMenu(SOTransition currentRecipe, float currentTimer, float maxTimer, bool isProducing, List<SOItems> itemAccepted)
    {
        isOpen = true;
        this.currentTimer = currentTimer;
        ActiveGeneratorMenu();
        generatorMenu.InitGeneratorMenu(currentRecipe, currentTimer, maxTimer, isProducing, itemAccepted);
        Player.Instance.CreateInventory(inventoryPanelGenerator, hoverImageGenerator, dividerGenerator, dragImageGenerator);
    }

    public void CloseMachineMenu()
    {
        Player.Instance.DestroyInventory();
        Clean();
        productionMenu.gameObject.SetActive(false);
        recipeMenu.gameObject.SetActive(false);
        
        isOpen = false;
    }

    public void Clean()
    {
        recipeMenu.CleanSelectRecipeMenu();
        productionMenu.CleanProductionMenu();
    }

    public void CloseDrillMenu()
    {
        Player.Instance.DestroyInventory();
        drillMenu.CleanDrillMenu();
        drillMenu.gameObject.SetActive(false);
    }

    public void CloseGeneratorMenu()
    {
        Player.Instance.DestroyInventory();
        generatorMenu.CleanGeneratorMenu();
        generatorMenu.gameObject.SetActive(false);
    }
    public void CloseMachineMenuWithCross()
    {
        Player.Instance.OpenCloseBuildingMenu();
        EventSystem.current.SetSelectedGameObject(null);
        isOpen = false;
    }

    public void ActiveRecipeMenu()
    {
        productionMenu.gameObject.SetActive(false);
        recipeMenu.gameObject.SetActive(true);
    }

    public void ActiveProductionMenu()
    {
        recipeMenu.gameObject.SetActive(false);
        productionMenu.gameObject.SetActive(true);
    }

    public void ActiveDrillMenu()
    {
        drillMenu.gameObject.SetActive(true);
    }

    public void ActiveGeneratorMenu()
    {
        generatorMenu.gameObject.SetActive(true);
    }
}
