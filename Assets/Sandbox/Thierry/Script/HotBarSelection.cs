using System.Collections.Generic;
using UnityEngine;

public class HotBarSelection : MonoBehaviour
{
    List<GameObject> tool = new List<GameObject>();
    [HideInInspector]
    public List<Item> toolItem = new List<Item>();
    private int index = -1;
    private int indexTool = 0;
    public int IndexTool { get => indexTool; }
    public bool isBuilding = false;

    private PlayerBuildance buildance;

    public BuildingState currentState = BuildingState.Exploration;

    public enum BuildingState
    {
        Exploration,
        Building,
        Destruction
    }
    void Start()
    {
        buildance = GetComponent<PlayerBuildance>();

        for (int i = 0; i < Player.Instance.GetNbHandSlots(); i++)
        {
            tool.Add(null);
            toolItem.Add(null);
        }

        InitInput();
        PlayerUi.Instance.UnselectAllItemHotbar();
        //Tool();
    }

    private void InitInput()
    {
        PlayerInputManager.Instance.HotBar1Action(() => Box(0), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar2Action(() => Box(1), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar3Action(() => Box(2), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar4Action(() => Box(3), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar5Action(() => Box(4), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar6Action(() => Box(5), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar7Action(() => Box(6), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar8Action(() => Box(7), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar9Action(() => Box(8), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBar10Action(() => Box(9), PlayerInputManager.ActionType.Add);

        PlayerInputManager.Instance.CancelBuildAction(Tool, PlayerInputManager.ActionType.Add);
    }

    public void UpdateHotBar()
    {
        SwitchTool();
    }

    void Box(int index)
    {
        if (index == this.index)
        {
            Tool();
        }
        else if (buildance.prefabsHotBar[index].Prefab != null)
        {
            this.index = index;
            isBuilding = true;
            ActivateBuildAtIndex(this.index);
        }
    }
    void SwitchTool()
    {
        if (!isBuilding)
        {
            float scrollValue = PlayerInputManager.Instance.GetMouseScrollValue().y;
            if (scrollValue > 0)
            {
                indexTool++;
                if (indexTool > tool.Count - 1)
                {
                    indexTool = 0;
                }
                Tool();
                UpdateUI();
                SetAnimationParameter();
            }
            else if (scrollValue < 0)
            {
                indexTool--;
                if (indexTool < 0)
                {
                    indexTool = tool.Count - 1;
                }
                Tool();
                UpdateUI();
                SetAnimationParameter();
            }
        }
    }

    private void Tool()
    {
        PlayerInputManager.Instance.DisableBuild();

        PlayerInputManager.Instance.EnableMenuing();
        PlayerInputManager.Instance.EnableDebugKeys();
        PlayerInputManager.Instance.EnableHudMachine();
        //PlayerInputManager.Instance.EnableCodex();
        PlayerInputManager.Instance.EnableAction();

        SelectTool();

        CloseHudBuild();
        if (currentState != BuildingState.Exploration)
        {
            PlayerUi.Instance.SwapInExplorationMode();
        }
        currentState = BuildingState.Exploration;
        ElecBallsManager.Instance.HideElecBalls();
        buildance.onDestructionMode = false;
    }

    public void CloseHudBuild()
    {
        buildance.CancelBuild();
        isBuilding = false;
        PlayerUi.Instance.DisableShortCutBuild();
        index = -1;
    }

    private void SelectTool()
    {
        for (int i = 0; i < tool.Count; i++)
        {
            if (tool[i] != null)
                tool[i].SetActive(false);
        }
        PlayerUi.Instance.UnselectAllItemHotbar();
        if (tool[indexTool] != null)
        {
            tool[indexTool].SetActive(true);
        }

        PlayerUi.Instance.SelectItemHand();
        index = -1;
    }

    private void ActivateBuildAtIndex(int index)
    {
        PlayerInputManager.Instance.DisableMenuing();
        PlayerInputManager.Instance.DisableDebugKeys();
        PlayerInputManager.Instance.DisableHudMachine();
        PlayerInputManager.Instance.DisableAction();
        PlayerInputManager.Instance.DisableCodex();

        PlayerInputManager.Instance.EnableCamera();
        PlayerInputManager.Instance.EnableBuild();
        PlayerInputManager.Instance.EnableInventory();
        PlayerInputManager.Instance.EnableMovement();

        buildance.ButtonHotbar(index);

        PlayerUi.Instance.UnselectAllItemHotbar();

        if (tool[indexTool] != null)
        {
            tool[indexTool].SetActive(false);
        }
        SOBuildingData buildingData = buildance.GetBuildingData();
        if (buildingData != null)
        {
            PlayerUi.Instance.ShortCutBuildUpdate(buildingData);
            if (currentState != BuildingState.Building)
            {
                PlayerUi.Instance.SwapInConstructionMode();
            }
            currentState = BuildingState.Building;
            ElecBallsManager.Instance.ShowElecBalls();
            buildance.onDestructionMode = false;
        }
        else
        {
            PlayerUi.Instance.DisableShortCutBuild();
        }
    }
    public void ActivateBuild()
    {
        PlayerInputManager.Instance.DisableMenuing();
        PlayerInputManager.Instance.DisableDebugKeys();
        PlayerInputManager.Instance.DisableHudMachine();
        PlayerInputManager.Instance.DisableAction();

        PlayerInputManager.Instance.EnableCamera();
        PlayerInputManager.Instance.EnableBuild();
        PlayerInputManager.Instance.EnableInventory();
        PlayerInputManager.Instance.EnableMovement();

        PlayerUi.Instance.UnselectAllItemHotbar();

        if (tool[indexTool] != null)
        {
            tool[indexTool].SetActive(false);
        }
        SOBuildingData buildingData = buildance.GetBuildingData();
        PlayerUi.Instance.ShortCutBuildUpdate(buildingData);
        if (currentState != BuildingState.Building)
        {
            PlayerUi.Instance.SwapInConstructionMode();
        }
        currentState = BuildingState.Building;
        ElecBallsManager.Instance.ShowElecBalls();
        buildance.onDestructionMode = false;
    }

    public void UpdateQuantityInInventory()
    {
        PlayerUi.Instance.ShortCutBuildUpdateCost();
    }

    public void AddHandItem(int _index, Item _item, GameObject _object = null)
    {
        tool[_index] = _object;
        toolItem[_index] = _item;
        SelectTool();
        UpdateUI();
        SetAnimationParameter();
    }

    public void RemoveHandItem(int _index)
    {
        tool[_index] = null;
        toolItem[_index] = null;
        UpdateUI();
        SetAnimationParameter();
    }
    private void UpdateUI()
    {
        if (toolItem[indexTool] != null)
        {
            PlayerUi.Instance.UpdateHandHotBar(toolItem[indexTool].itemType.name, toolItem[indexTool].itemType.Sprite);
        }
        else
        {
            PlayerUi.Instance.HideHandUi();
        }
    }

    private void SetAnimationParameter()
    {
        if (tool[indexTool] == null)
        {
            Player.Instance.SetBoolAnimator("IsHandEmpty", true);
            Player.Instance.SetBoolAnimator("IsHoldingGun", false);
            Player.Instance.SetBoolAnimator("IsHoldingTaser", false);
        }
        else
        {
            if (toolItem[indexTool].itemType != null && toolItem[indexTool].itemType.name == "Taser")
            {
                Player.Instance.SetBoolAnimator("IsHandEmpty", false);
                Player.Instance.SetBoolAnimator("IsHoldingGun", false);
                Player.Instance.SetBoolAnimator("IsHoldingTaser", true);
            }
            else if (toolItem[indexTool].itemType != null && toolItem[indexTool].itemType.name == "Rifle")
            {
                Player.Instance.SetBoolAnimator("IsHandEmpty", false);
                Player.Instance.SetBoolAnimator("IsHoldingGun", true);
                Player.Instance.SetBoolAnimator("IsHoldingTaser", false);
            }
        }
    }
}