using UnityEngine;
using UnityEngine.UI;

public class SaveLoadCheat : MonoBehaviour
{
    [Header("Save Load")]
    [SerializeField] private Button save;
    [SerializeField] private Button load;
    [SerializeField] private Toggle autoSave;
    [SerializeField] private Button reset;

    private bool defaultAutoSave = true;

    private void Start()
    {
        autoSave.isOn = defaultAutoSave;
        autoSave.onValueChanged.AddListener(ToggleAutoSave);

        save.onClick.AddListener(SaveSystem.Instance.Save);
        load.onClick.AddListener(SaveSystem.Instance.Load);
        reset.onClick.AddListener(ResetToDefaultValue);
    }

    private void ToggleAutoSave(bool value)
    {

    }

    private void ResetToDefaultValue()
    {
        autoSave.isOn = defaultAutoSave;
    }
}