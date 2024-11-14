using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MilestoneCost : HoverUI
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI value;

    public override HoverData HoverData
    {
        get => base.HoverData;
        set
        {
            base.HoverData = value;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        icon.sprite = hoverData.icon;
        value.text = hoverData.value.ToString();
    }

    public void Disable() => gameObject.SetActive(false);
    public void Enable() => gameObject.SetActive(true);

}