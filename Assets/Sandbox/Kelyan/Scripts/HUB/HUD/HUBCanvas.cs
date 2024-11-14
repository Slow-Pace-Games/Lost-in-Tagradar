using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//desac le canvas en entier pour évité le raycast en continue
//alors le last state avec le destroy inventory j'ai envie de caner mais avec les possible modif de display d'inventory sa ira ptet mieux
public class HUBCanvas : MonoBehaviour
{
    #region Singleton
    private static HUBCanvas instance;
    public static HUBCanvas Instance { get => instance; }

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

    private enum State
    {
        Craft,
        Milestone,
        ActiveMilestone,
    }

    [Header("Managers")]
    [SerializeField] private CraftBench craftBench;
    [SerializeField] private SelectMilestone selectMilestone;
    [SerializeField] private ActiveMilestone activeMilestone;
    [SerializeField] private State state;
    private State lastMenu;

    private void Start()
    {
        craftBench.InitCraftBench();
        selectMilestone.InitSelectMilestone();
        activeMilestone.InitActiveMilestone();
    }

    public void OpenHUBCanvas()
    {
        switch (state)
        {
            case State.Craft:
                OpenCraftBenchMenu();
                lastMenu = State.Craft;
                break;
            case State.Milestone:
                OpenMilestoneMenu();
                lastMenu = State.Milestone;
                break;
            case State.ActiveMilestone:
                OpenActiveMilestoneMenu();
                lastMenu = State.ActiveMilestone;
                break;

            default:
                Debug.LogError("No state");
                break;
        }
    }

    public void CloseHUBCanvas()
    {
        if (lastMenu == State.Craft)
        {
            craftBench.DestroyInventory();
        }
        else if (lastMenu == State.ActiveMilestone)
        {
            activeMilestone.DestroyInventory();
        }
        PlayerInputManager.Instance.DisableCrafting();
        craftBench.gameObject.SetActive(false);
        selectMilestone.gameObject.SetActive(false);
        activeMilestone.gameObject.SetActive(false);
    }

    public void CloseHUBCanvasWithCross()
    {
        Player.Instance.OpenCloseBuildingMenu();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenCraftBenchMenu()
    {
        if (lastMenu == State.ActiveMilestone)
        {
            activeMilestone.DestroyInventory();
        }

        craftBench.OpenCraftBench();
        state = State.Craft;
        lastMenu = state;
        selectMilestone.gameObject.SetActive(false);
        activeMilestone.gameObject.SetActive(false);
    }

    public void OpenMilestoneMenu()
    {
        if (lastMenu == State.Craft)
        {
            craftBench.DestroyInventory();
        }
        else if (lastMenu == State.ActiveMilestone)
        {
            activeMilestone.DestroyInventory();
        }
        selectMilestone.OpenSelectionMilestone();
        state = State.Milestone;
        lastMenu = state;
        craftBench.gameObject.SetActive(false);
        activeMilestone.gameObject.SetActive(false);
    }

    public void OpenActiveMilestoneMenu()
    {
        if (lastMenu == State.Craft)
        {
            craftBench.DestroyInventory();
        }
        activeMilestone.OpenActiveMilestone();
        state = State.ActiveMilestone;
        lastMenu = state;
        craftBench.gameObject.SetActive(false);
        selectMilestone.gameObject.SetActive(false);
    }

    public void SetNewUpgrade(SOTier newUpgrade, int currentMilestone, int CurrentTierUpgrade)
    {
        activeMilestone.ActiveNewUpgrade(newUpgrade, currentMilestone, CurrentTierUpgrade);
    }

    public void UnlockNewRecipes(List<SORecipe> recipes)
    {
        craftBench.UnlockNewRecipes(recipes);
    }

    #region SelectMilestone
    public void UpdateHoverSelectMilestone(HoverData hoverData) => selectMilestone.HoverMilestoneData(hoverData);
    public void ExitHoverSelectMilestone() => selectMilestone.ExitHoverMilesoneData();
    #endregion
}