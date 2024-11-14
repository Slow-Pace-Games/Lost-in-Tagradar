using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebind : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionRef;
    [SerializeField] private InputActionReference secondaryInputActionRef;
    [SerializeField] private int selectedBinding;
    [SerializeField] private int secondaryBinding;
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;

    private string actionName;

    [Header("UI")]
    [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text rebindText;
    [SerializeField] Button rebindButton;

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());

        GetBindingInfo();
        UpdateUI();

        RebindManager.rebindComplete += UpdateUI;
        RebindManager.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        RebindManager.rebindComplete -= UpdateUI;
    }

    private void GetBindingInfo()
    {
        actionName = inputActionRef.action.name;
    }

    private void UpdateUI()
    {
        rebindText.text = RebindManager.GetBindingName(actionName, selectedBinding);

        if (inputActionRef.action.name == "ZQSD")
        {
            actionText.text = inputActionRef.action.bindings[selectedBinding].name.FirstCharacterToUpper();
            return;
        }
        actionText.text = actionName;
    }

    private void DoRebind()
    {
        RebindManager.StartRebind(actionName, selectedBinding, rebindText, secondaryInputActionRef, secondaryBinding);
    }
}
