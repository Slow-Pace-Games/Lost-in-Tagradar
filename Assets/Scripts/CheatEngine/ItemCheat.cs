using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCheat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameItem;
    private Image bg;
    private bool selected;
    private InventoryCheat.ItemClickEvent onClickEvent;
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Normal);
        }
    }

    public void Init(Sprite itemSprite, string name, InventoryCheat.ItemClickEvent onClickEvent)
    {
        bg = GetComponent<Image>();
        bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Normal);

        icon.sprite = itemSprite;
        nameItem.text = name;

        selected = false;

        this.onClickEvent = onClickEvent;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selected)
        {
            selected = true;
            bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Select);
            onClickEvent.Invoke(transform.GetSiblingIndex());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
            bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Enter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
            bg.color = CheatEngine.Instance.TabColor(CheatEngine.TabState.Normal);
    }
}