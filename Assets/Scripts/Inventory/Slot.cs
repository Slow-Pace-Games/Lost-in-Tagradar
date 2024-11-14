using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected Item item;
    public Item Item { get => item; set => item = value; }

    [Header("UI")]
    [SerializeField] protected Image icon;
    [SerializeField] protected Image bg;
    [SerializeField] protected Image bgStack;
    [SerializeField] protected TextMeshProUGUI stacksText;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite highlightedSprite;

    protected bool isDroppingOnSameSpot = false;

    public void UpdateSlot(Item _item)
    {
        item = _item;
        UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        if (item != null && item.stacks <= 0)
        {
            item = null;
            HideUI();
            return;
        }

        if (item == null)
        {
            HideUI();
            return;
        }

        stacksText.enabled = true;
        bgStack.enabled = true;
        ColorUtility.TryParseHtmlString("#A2A2A2", out Color myColor);
        bg.color = myColor;
        icon.sprite = item.itemType.Sprite;
        icon.color = Color.white;
        stacksText.text = item.stacks.ToString();
    }

    protected void HideUI()
    {
        icon.sprite = null;
        icon.color = Color.clear;
        stacksText.enabled = false;
        bgStack.enabled = false;
        ColorUtility.TryParseHtmlString("#4D4D4D", out Color myColor);
        bg.color = myColor;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            Player.Instance.SetBaseDragItem(item);
            item = null;
            Player.Instance.SetDragSprite(icon.sprite);
            HideUI();
            Player.Instance.ShowMouseHover(false);
            isDroppingOnSameSpot = false;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Player.Instance.FollowMouse();
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        Item baseItem = Player.Instance.GetBaseDragItem();
        if (baseItem == null)
        {
            return;
        }

        if (item != null && item.itemType == baseItem.itemType)
        {
            int maxStack = 100;
            if (item.stacks + baseItem.stacks <= maxStack)
            {
                item.stacks += baseItem.stacks;
                Player.Instance.SetBaseDragItem(null);
            }
            else
            {
                int stacksToAdd = maxStack - item.stacks;
                item.stacks += stacksToAdd;
                Player.Instance.GetBaseDragItem().stacks -= stacksToAdd;
            }
        }
        else
        {
            Player.Instance.SetBaseDragItem(item);
            item = baseItem;
        }
        isDroppingOnSameSpot = true;
        UpdateUI();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {

    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        bg.sprite = highlightedSprite;
        if (item != null)
        {
            StartCoroutine(ShowTooltip());
        }
    }



    public virtual void OnPointerExit(PointerEventData eventData)
    {
        bg.sprite = normalSprite;
        StopCoroutine(ShowTooltip());
        Player.Instance.ShowMouseHover(false);
    }

    IEnumerator ShowTooltip()
    {
        yield return new WaitForSeconds(0.5f);
        if (item != null)
        {
            Player.Instance.ShowMouseHover(true, item.itemType, transform.position);
        }
        yield return null;
    }
}