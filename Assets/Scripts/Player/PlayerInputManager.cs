using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region Singleton
    [SerializeField] SOInputs SOInputs;
    private static PlayerInputManager instance;
    public static PlayerInputManager Instance { get => instance; }

    public PlayerInputActions inputActions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            inputActions = SOInputs.inputActions;
            inputActions.Enable();

            DisableBuildMenu();
            DisableBuild();
            DisableInventory();
            DisableCodex();
            DisableMenuing();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Main
    public void DisablePauseMenu() => inputActions.Action.OpenPause.Disable();
    public void EnablePauseMenu() => inputActions.Action.OpenPause.Enable();

    public enum ActionType
    {
        Add,
        Remove,
    }
    public void EnableInput()
    {
        inputActions.Enable();
    }
    public void DisableInput()
    {
        inputActions.Disable();
    }
    private void OnDestroy()
    {
        SOInputs.inputActions.Dispose();
        SOInputs.inputActions.Disable();

        inputActions.Disable();
        inputActions.Dispose();

        SOInputs.Destroy();

        inputActions = null;
    }
    #endregion

    #region Player
    public Vector2 GetZQSDMovementsValue()
    {
        return inputActions.Movements.ZQSD.ReadValue<Vector2>();
    }
    public Vector2 GetCameraMovementsValue()
    {
        return inputActions.Movements.MouseMovements.ReadValue<Vector2>();
    }
    public void CrouchAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Movements.Crouch.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Movements.Crouch.performed -= ctx => ftc();
        }
    }
    public void RunAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Movements.Run.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Movements.Run.performed -= ctx => ftc();
        }
    }
    public void JumpAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Movements.Jump.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Movements.Jump.performed -= ctx => ftc();
        }
    }
    public bool CrouchIsPressed()
    {
        return inputActions.Movements.Crouch.IsPressed();
    }
    public bool JumpIsPressed()
    {
        return inputActions.Movements.Jump.IsPressed();
    }
    public bool RunIsPressed()
    {
        return inputActions.Movements.Run.IsPressed();
    }
    public void DisableMovement()
    {
        inputActions.Movements.Disable();
    }
    public void EnableMovement()
    {
        inputActions.Movements.Enable();
    }
    public void DisableCamera()
    {
        inputActions.Movements.MouseMovements.Disable();
    }
    public void EnableCamera()
    {
        inputActions.Movements.MouseMovements.Enable();
    }
    #endregion

    #region Action
    public void InteractAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.Interact.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Action.Interact.performed -= ctx => ftc();
        }
    }
    public void OpenInventoryAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.OpenInventory.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Action.OpenInventory.performed -= ctx => ftc();
        }
    }
    public void AttackAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.Attack.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Action.Attack.performed -= ctx => ftc();
        }
    }
    public void BuildMenuAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.OpenBuildMenu.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Action.OpenBuildMenu.performed -= ctx => ftc();
        }
    }

    public void SelectRessourceScan(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.Scan.started += ctx => ftc();
        }
    }

    public void StartScanAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.Scan.canceled += ctx => ftc();
        }
    }

    public bool InteractIsPressed()
    {
        return inputActions.Action.Interact.IsPressed();
    }
    public string InteractKeyName()
    {
        return inputActions.Action.Interact.GetBindingDisplayString(0);
    }
    public string OpenInventoryKeyName()
    {
        return inputActions.Action.OpenInventory.GetBindingDisplayString(0);
    }
    public string OpenCodexKeyName()
    {
        return inputActions.Action.OpenCodex.GetBindingDisplayString(0);
    }
    public string ScannerKeyName()
    {
        return inputActions.Action.Scan.GetBindingDisplayString(0);
    }
    public bool IsActionEnable()
    {
        return inputActions.Action.enabled;
    }
    public void EnableAction()
    {
        inputActions.Action.Enable();
    }
    public void DisableAction()
    {
        inputActions.Action.Disable();
    }
    #endregion

    #region Inventory
    public void CloseInventoryAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Inventory.CloseInventory.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Inventory.CloseInventory.performed -= ctx => ftc();
        }
    }
    public void DivideStacksCloseAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Inventory.DivideStacks.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Inventory.DivideStacks.performed -= ctx => ftc();
        }
    }
    public void DisableInventory()
    {
        inputActions.Inventory.Disable();
    }
    public void EnableInventory()
    {
        inputActions.Inventory.Enable();
    }
    #endregion

    #region Build
    public Vector2 GetMouseScrollValue()
    {
        return inputActions.Build.MouseScroll.ReadValue<Vector2>();
    }
    public void BuildAction(Action fct, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Build.performed += ctx => fct();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Build.performed -= ctx => fct();
        }
    }
    public void CancelBuildAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.CancelBuild.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.CancelBuild.performed -= ctx => ftc();
        }
    }
    public void HotBar1Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar1.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar1.performed -= ctx => ftc();
        }
    }
    public void HotBar2Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar2.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar2.performed -= ctx => ftc();
        }
    }
    public void HotBar3Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar3.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar3.performed -= ctx => ftc();
        }
    }
    public void HotBar4Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar4.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar4.performed -= ctx => ftc();
        }
    }
    public void HotBar5Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar5.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar5.performed -= ctx => ftc();
        }
    }
    public void HotBar6Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar6.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar6.performed -= ctx => ftc();
        }
    }
    public void HotBar7Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar7.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar7.performed -= ctx => ftc();
        }
    }
    public void HotBar8Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar8.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar8.performed -= ctx => ftc();
        }
    }
    public void HotBar9Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar9.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar9.performed -= ctx => ftc();
        }
    }
    public void HotBar10Action(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Hotbar10.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Hotbar10.performed -= ctx => ftc();
        }
    }
    public void DestructAction(Action fct, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.Destruct.performed += ctx => fct();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.Destruct.performed -= ctx => fct();
        }
    }
    public void DisableBuild()
    {
        inputActions.Build.Build.Disable();
        inputActions.Build.CancelBuild.Disable();
    }
    public void EnableBuild()
    {
        inputActions.Build.Enable();
    }
    public void DisableHotbarAndBuild()
    {
        inputActions.Build.Disable();
    }
    public void EnableHotbarAndBuild()
    {
        inputActions.Build.Enable();
    }
    public string DestructKeyName()
    {
        return inputActions.Build.Destruct.GetBindingDisplayString(0);
    }
    public void DestructionMode(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Build.DestructionMode.started += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Build.DestructionMode.started -= ctx => ftc();
        }
    }

    public string DestructionModeKeyName()
    {
        return inputActions.Build.DestructionMode.GetBindingDisplayString(0);
    }
    #endregion

    #region Hud Machine
    public void CloseAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.HudMachine.Close.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.HudMachine.Close.performed -= ctx => ftc();
        }
    }
    public void InteractMachineHudAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.HudMachine.InteractMachineHud.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.HudMachine.InteractMachineHud.performed -= ctx => ftc();
        }
    }
    public void DisableHudMachine()
    {
        inputActions.HudMachine.Disable();
    }
    public void EnableHudMachine()
    {
        inputActions.HudMachine.Enable();
    }
    #endregion

    #region Menuing
    public void ClosePauseMenuAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Menuing.Close.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Menuing.Close.performed -= ctx => ftc();
        }
    }
    public void OpenPauseMenuAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.OpenPause.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.Action.OpenPause.performed -= ctx => ftc();
        }
    }
    public void DisableMenuing()
    {
        inputActions.Menuing.Disable();
    }
    public void EnableMenuing()
    {
        inputActions.Menuing.Enable();
    }
    #endregion

    #region Debug Keys
    public void SaveAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.Save.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.Save.performed -= ctx => ftc();
        }
    }
    public void LoadAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.Load.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.Load.performed -= ctx => ftc();
        }
    }
    public void AddSlotAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.AddSlot.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.AddSlot.performed -= ctx => ftc();
        }
    }
    public void AddItemAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.AddItem.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.AddItem.performed -= ctx => ftc();
        }
    }
    public void UnlockAllBuildingAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.UnlockAllBuilding.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.UnlockAllBuilding.performed -= ctx => ftc();
        }
    }
    public void CheatEngineAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.CheatEngine.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.CheatEngine.performed -= ctx => ftc();
        }
    }
    public void StartTravelingAction(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.DebugKeys.StartTraveling.performed += ctx => ftc();
        }
        else if (action == ActionType.Remove)
        {
            inputActions.DebugKeys.StartTraveling.performed -= ctx => ftc();
        }
    }
    public void DisableDebugKeys()
    {
        inputActions.DebugKeys.Disable();
    }
    public void EnableDebugKeys()
    {
        inputActions.DebugKeys.Enable();
    }
    #endregion

    #region CraftBench

    public bool IsCraftKeyboardPressed()
    {
        return inputActions.CraftBench.CraftWithKeyboard.IsPressed();
    }

    public bool IsCraftMousePressed()
    {
        return inputActions.CraftBench.CraftWithMouse.IsPressed();
    }

    public void DisableCrafting()
    {
        inputActions.CraftBench.Disable();
    }
    public void EnableCrafting()
    {
        inputActions.CraftBench.Enable();
    }
    #endregion

    #region Codex
    public void OpenCodex(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Action.OpenCodex.started += ctx => ftc();
        }
    }
    public void CloseCodex(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.Codex.CloseCodex.started += ctx => ftc();
        }
    }
    public void DisableCodex()
    {
        inputActions.Codex.Disable();
    }
    public void EnableCodex()
    {
        inputActions.Codex.Enable();
    }
    #endregion

    #region BuildMenu
    public void CloseBuildMenu(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.CloseBuildMenu.started += ctx => ftc();
        }
    }
    public void HotBarBinding1(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar1.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar1.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding2(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar2.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar2.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding3(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar3.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar3.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding4(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar4.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar4.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding5(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar5.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar5.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding6(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar6.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar6.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding7(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar7.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar7.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding8(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar8.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar8.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding9(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar9.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar9.performed -= ctx => ftc();
        }
    }
    public void HotBarBinding10(Action ftc, ActionType action)
    {
        if (action == ActionType.Add)
        {
            inputActions.BuildMenu.Hotbar10.performed += ctx => ftc();
        }
        else
        {
            inputActions.BuildMenu.Hotbar10.performed -= ctx => ftc();
        }
    }
    public void DisableBuildMenu()
    {
        inputActions.BuildMenu.Disable();
    }
    public void EnableBuildMenu()
    {
        inputActions.BuildMenu.Enable();
    }
    public void EnableBuildMenuComponents()
    {
        inputActions.BuildMenu.Hotbar1.Enable();
        inputActions.BuildMenu.Hotbar2.Enable();
        inputActions.BuildMenu.Hotbar3.Enable();
        inputActions.BuildMenu.Hotbar4.Enable();
        inputActions.BuildMenu.Hotbar5.Enable();
        inputActions.BuildMenu.Hotbar6.Enable();
        inputActions.BuildMenu.Hotbar7.Enable();
        inputActions.BuildMenu.Hotbar8.Enable();
        inputActions.BuildMenu.Hotbar9.Enable();
        inputActions.BuildMenu.Hotbar10.Enable();
    }
    public void DisableBuildMenuComponents()
    {
        inputActions.BuildMenu.Hotbar1.Disable();
        inputActions.BuildMenu.Hotbar2.Disable();
        inputActions.BuildMenu.Hotbar3.Disable();
        inputActions.BuildMenu.Hotbar4.Disable();
        inputActions.BuildMenu.Hotbar5.Disable();
        inputActions.BuildMenu.Hotbar6.Disable();
        inputActions.BuildMenu.Hotbar7.Disable();
        inputActions.BuildMenu.Hotbar8.Disable();
        inputActions.BuildMenu.Hotbar9.Disable();
        inputActions.BuildMenu.Hotbar10.Disable();
    }
    public string OpenBuildGetNameKey()
    {
        return inputActions.Action.OpenBuildMenu.GetBindingDisplayString(0);
    }
    #endregion
}