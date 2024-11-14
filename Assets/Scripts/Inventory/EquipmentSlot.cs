using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    private int index;
    private Item draggedItem;

    public void Init(int _i)
    {
        stacksText = GetComponentInChildren<TextMeshProUGUI>();
        index = _i;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        draggedItem = item;
        base.OnBeginDrag(eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Item baseItem = Player.Instance.GetBaseDragItem();
        if (baseItem == null)
        {
            return;
        }

        if (baseItem.itemType.isEquipable && baseItem.itemType.gameObject != null && index <= 2)
        {
            Player.Instance.SetBaseDragItem(item);
            item = baseItem;

            Transform rightHand = Player.Instance.GetRightHand();
            if (rightHand.GetChild(index).childCount != 0)
            {
                Destroy(rightHand.GetChild(index).GetChild(0).gameObject);
            }

            if (item != null)
            {
                GameObject go = Instantiate(item.itemType.gameObject, rightHand.GetChild(index));
                Player.Instance.AddHandItem(index, item, go);
            }
            else
            {
                Player.Instance.AddHandItem(index, item);
            }
        }
        else if (baseItem.itemType.gameObject == null && index >= 3)
        {
            Player.Instance.SetBaseDragItem(item);
            item = baseItem;
            Player.Instance.UpdateEquipments(index - Player.Instance.GetNbHandSlots(), item);

            EquipItem();
        }

        UpdateUI();
        isDroppingOnSameSpot = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        Item baseItem = Player.Instance.GetBaseDragItem();
        if (baseItem == null || baseItem.itemType.isEquipable)
        {
            if (index < 3)
            {
                if (!isDroppingOnSameSpot)
                {
                    item = baseItem;
                }

                Transform rightHand = Player.Instance.GetRightHand();
                if (item == null)
                {
                    Player.Instance.RemoveHandItem(index);
                    Destroy(rightHand.GetChild(index).GetChild(0).gameObject);
                }
                else
                {
                    Destroy(rightHand.GetChild(index).GetChild(0).gameObject);
                    GameObject go = Instantiate(item.itemType.gameObject, rightHand.GetChild(index));
                    Player.Instance.AddHandItem(index, item, go);
                }
            }
            else
            {
                UnequipItem();

                if (!isDroppingOnSameSpot)
                {
                    item = baseItem;
                }

                Player.Instance.UpdateEquipments(index - Player.Instance.GetNbHandSlots(), item);
                draggedItem = null;
                EquipItem();
            }

            Player.Instance.SetDragSprite(null);
            UpdateUI();
        }
    }

    private void EquipItem()
    {
        if (item != null)
        {
            if (item.itemType.name == "Armor")
            {
                Player.Instance.SetHasArmor(true);
            }
            else if (item.itemType.name == "Battery")
            {
                Player.Instance.SetRegenTickCooldown(3f);
            }
            else if (item.itemType.name == "Boots")
            {
                Player.Instance.SetSpeedMultiplier(1.5f);
            }
        }
    }

    private void UnequipItem()
    {
        if (draggedItem != null)
        {
            if (draggedItem.itemType.name == "Armor")
            {
                Player.Instance.SetHasArmor(false);
            }
            else if (draggedItem.itemType.name == "Battery")
            {
                Player.Instance.SetRegenTickCooldown(5f);
            }
            else if (draggedItem.itemType.name == "Boots")
            {
                Player.Instance.SetSpeedMultiplier(1f);
            }
        }
    }
}