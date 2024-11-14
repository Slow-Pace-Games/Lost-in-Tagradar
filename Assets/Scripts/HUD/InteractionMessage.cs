using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image background;

    private string interact;
    private string destroy;

    private void Start()
    {
        SetTextDisable();

        RessourcesRayManager.instance.OnDeselectObject += SetTextDisable;

        UpdateKeyNames();
    }

    public void SetRessourceTextEnable(string selectedObjectName)
    {
        text.text = "Hold " + interact.ToUpper() + " to pick up " + selectedObjectName;
        SetTextEnable();
    }
    public void SetUnpickableRessourceTextEnable(string selectedObjectName)
    {
        text.text = "Not enough place to pick up " + selectedObjectName;
        SetTextEnable();
    }
    public void SetMachineTextEnable(string selectedObjectName)
    {
        text.text = "Press " + interact.ToUpper() + " to configure " + selectedObjectName;
        SetTextEnable();
    }
    public void SetDestructTextEnable(string selectedObjectName)
    {
        text.text = "Press " + destroy.ToUpper() + " to destroy " + selectedObjectName;
        SetTextEnable();
    }
    public void SetToolTextEnable(bool enable)
    {
        text.text = "Press " + interact.ToUpper() + " to use";
        SetTextEnable();
    }
    void SetTextEnable()
    {
        text.enabled = true;
        background.enabled = true;
    }
    void SetTextDisable()
    {
        text.enabled = false;
        background.enabled = false;
    }

    public void UpdateKeyNames()
    {
        interact = PlayerInputManager.Instance.InteractKeyName();
        destroy = PlayerInputManager.Instance.DestructKeyName();
    }
}