using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] Image icon;
    [SerializeField] RectTransform rectTransform;

    public void SetHandInfo(string _name, Sprite _icon)
    {
        ShowUI();
        itemName.text = _name;
        icon.sprite = _icon;
    }

    public void HideUI()
    {
        itemName.enabled = false;
        icon.enabled = false;
    }

    private void ShowUI()
    {
        itemName.enabled = true;
        icon.enabled = true;
    }

    public void Select()
    {
        rectTransform.sizeDelta = new Vector2(80f, 80f);
    }

    public void Unselect()
    {
        rectTransform.sizeDelta = new Vector2(70f, 70f);
    }
}
