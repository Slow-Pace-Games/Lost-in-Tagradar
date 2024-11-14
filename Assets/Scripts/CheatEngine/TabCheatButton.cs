using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabCheatButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    [Header("Window")]
    [SerializeField] private GameObject window;
    private Image bg;

    private bool isSelected = false;

    public void Init()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = gameObject.name;
        bg = GetComponentInChildren<Image>();
        bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Normal);
    }
    public void OpenTabWindow()
    {
        isSelected = true;
        bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Select);

        window.SetActive(true);

    }
    public void CloseTabWindow()
    {
        isSelected = false;
        bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Normal);

        window.SetActive(false);
    }

    #region IPointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Enter);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Select);
        CheatEngine.Instance.onClickEvent.Invoke((CheatEngine.CheatState)transform.GetSiblingIndex());

        isSelected = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Normal);
        }
    }
    #endregion
}