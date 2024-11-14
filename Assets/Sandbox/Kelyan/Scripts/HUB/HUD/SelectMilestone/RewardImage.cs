using UnityEngine;
using UnityEngine.UI;

public class RewardImage : HoverUI
{
    [SerializeField] private Image icon;

    public override HoverData HoverData
    {
        get => base.HoverData;
        set
        {
            base.HoverData = value;
            UpdateUI();
        }
    }

    private void UpdateUI() => icon.sprite = hoverData.icon;
    public void Disable() => gameObject.SetActive(false);
    public void Enable() => gameObject.SetActive(true);
}