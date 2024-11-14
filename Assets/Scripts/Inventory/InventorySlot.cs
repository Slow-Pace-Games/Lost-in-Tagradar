using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : Slot
{
    private int index;

    public void Init(int _i)
    {
        stacksText = GetComponentInChildren<TextMeshProUGUI>();
        index = _i;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        Player.Instance.UpdateInventory(index, item);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!isDroppingOnSameSpot)
        {
            item = Player.Instance.GetBaseDragItem();
        }
        Player.Instance.UpdateInventory(index, item);
        Player.Instance.SetDragSprite(null);
        UpdateUI();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                Player.Instance.OnClickInventorySlot(index, item, transform.position);
            }
        }
    }
}