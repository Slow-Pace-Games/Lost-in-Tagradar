using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputOutputSlot : Slot
{
    [SerializeField] SOItems currentItem;
    [SerializeField] List<SOItems> itemsAccepted;
    int stacks;

    public void Init(SOItems _currentItem, List<SOItems> _items = null)
    {
        stacksText = GetComponentInChildren<TextMeshProUGUI>();
        currentItem = _currentItem;
        itemsAccepted = _items;

        HideUI();
    }

    public void UpdateSlot(int _stacks)
    {
        stacks = _stacks;
        UpdateUI();
    }

    protected override void UpdateUI()
    {
        if (stacks <= 0)
        {
            HideUI();
            return;
        }

        stacksText.enabled = true;
        bgStack.enabled = true;
        ColorUtility.TryParseHtmlString("#A2A2A2", out Color myColor);
        bg.color = myColor;
        icon.sprite = currentItem.Sprite;
        icon.color = Color.white;
        stacksText.text = stacks.ToString();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (stacks != 0)
        {
            Item newItem = new Item();
            newItem.itemType = currentItem;
            newItem.stacks = stacks;
            Player.Instance.SetBaseDragItem(newItem);
            Player.Instance.SetDragSprite(icon.sprite);

            HideUI();

            bool isTargetItemInInput = true;
            if (transform.parent.parent.name == "ContainerOutput")
            {
                isTargetItemInInput = false;
                MachineHUDManager.Instance.onStackValueChange?.Invoke(-stacks, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
            }
            else if (transform.parent.parent.name == "ContainerInputs")
            {
                MachineHUDManager.Instance.onStackValueChange?.Invoke(-stacks, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
            }

            stacks = 0;
            isDroppingOnSameSpot = false;
        }
        else
        {
            Player.Instance.SetBaseDragItem(null);
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Item baseItem = Player.Instance.GetBaseDragItem();

        if (baseItem == null)
        {
            return;
        }

        bool isTargetItemInInput = true;
        if (transform.parent.parent.name == "ContainerOutput")
        {
            isTargetItemInInput = false;
        }

        //SAme item dropped as the one in the slot
        if (currentItem == baseItem.itemType)
        {
            int maxStack = 100;
            if (stacks + baseItem.stacks <= maxStack)
            {
                stacks += baseItem.stacks;
                Player.Instance.SetBaseDragItem(null);
                MachineHUDManager.Instance.onStackValueChange?.Invoke(baseItem.stacks, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
            }
            else
            {
                int stacksToAdd = maxStack - stacks;
                stacks += stacksToAdd;
                Player.Instance.GetBaseDragItem().stacks -= stacksToAdd;
                MachineHUDManager.Instance.onStackValueChange?.Invoke(stacksToAdd, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
            }
        }
        else if (itemsAccepted != null && itemsAccepted.Count > 1 && stacks == 0)
        {
            foreach (SOItems item in itemsAccepted)
            {
                if (item == baseItem.itemType)
                {
                    int maxStack = 100;
                    if (stacks + baseItem.stacks <= maxStack)
                    {
                        stacks += baseItem.stacks;
                        Player.Instance.SetBaseDragItem(null);
                        MachineHUDManager.Instance.onStackValueChange?.Invoke(baseItem.stacks, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
                    }
                    else
                    {
                        int stacksToAdd = maxStack - stacks;
                        stacks += stacksToAdd;
                        Player.Instance.GetBaseDragItem().stacks -= stacksToAdd;
                        MachineHUDManager.Instance.onStackValueChange?.Invoke(stacksToAdd, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
                    }
                    currentItem = baseItem.itemType;
                    MachineHUDManager.Instance.onTransitionChange?.Invoke(baseItem.itemType);

                    GetComponentInParent<ItemHUD>().stack = stacks;
                    UpdateUI();
                    return;
                }
            }
        }

        isDroppingOnSameSpot = true;
        GetComponentInParent<ItemHUD>().stack = stacks;
        UpdateUI();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        bool isTargetItemInInput = true;
        if (transform.parent.parent.name == "ContainerOutput")
        {
            isTargetItemInInput = false;
        }

        if (!isDroppingOnSameSpot)
        {
            Item baseItem = Player.Instance.GetBaseDragItem();
            stacks = baseItem != null ? baseItem.stacks : 0;
        }

        MachineHUDManager.Instance.onStackValueChange?.Invoke(stacks, isTargetItemInInput, transform.parent.GetSiblingIndex(), currentItem);
        Player.Instance.SetDragSprite(null);
        UpdateUI();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    public override void OnPointerExit(PointerEventData eventData)
    {

    }
}