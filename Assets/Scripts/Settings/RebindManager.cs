using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindManager : MonoBehaviour
{
    [SerializeField] SOInputs SOInputs;
    private static PlayerInputActions inputActions;

    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;


    private void Awake()
    {
        inputActions = SOInputs.inputActions;
    }

    public static void StartRebind(string _actionName, int _bindingIndex, TMP_Text _statusText, InputAction _secondaryAction, int _secondaryBindingIndex)
    {
        InputAction action = inputActions.asset.FindAction(_actionName);

        DoRebind(action, _bindingIndex, _statusText);

        if (_secondaryAction != null)
        {
            InputAction secondaryAction = inputActions.asset.FindAction(_secondaryAction.name);
            RebindSecondaryAction(secondaryAction, _secondaryBindingIndex);
        }
    }

    public static void DoRebind(InputAction _actionToRebind, int _bindingIndex, TMP_Text _statusText)
    {
        _statusText.text = "Press any key";

        _actionToRebind.Disable();

        var rebind = _actionToRebind.PerformInteractiveRebinding(_bindingIndex);

        rebind.OnComplete(operation =>
        {
            _actionToRebind.Enable();
            operation.Dispose();
            rebindComplete?.Invoke();
            if (PlayerUi.Instance != null)
            {
                PlayerUi.Instance.UpdateShortCut();
                PlayerUi.Instance.UpdateKeyNamesInteraction();
            }
        });

        rebind.OnCancel(operation =>
        {
            _actionToRebind.Enable();
            operation.Dispose();
            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        rebindStarted?.Invoke(_actionToRebind, _bindingIndex);
        rebind.Start();
    }

    private static void RebindSecondaryAction(InputAction _action, int _bindingIndex)
    {
        _action.Disable();
        
        var rebind = _action.PerformInteractiveRebinding(_bindingIndex);

        rebind.OnComplete(operation =>
        {
            _action.Enable();
            operation.Dispose();
        });

        rebind.OnCancel(operation =>
        {
            _action.Enable();
            operation.Dispose();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        rebindStarted?.Invoke(_action, _bindingIndex);
        rebind.Start();

    }

    public static string GetBindingName(string _actionName, int _bindingIndex)
    {
        InputAction action = inputActions.asset.FindAction(_actionName);
        return action.GetBindingDisplayString(_bindingIndex);
    }

    public void ResetBindings()
    {
        inputActions.asset.RemoveAllBindingOverrides();
        rebindComplete?.Invoke();
    }
}