using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

[Serializable]
public class Item
{
    public SOItems itemType;
    public int stacks;
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] SODatabase database;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject equipmentSlotPrefab;
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] Transform inventorySlotContainer;
    [SerializeField] Transform inventoryHover;
    [SerializeField] Transform inventoryDivider;
    [SerializeField] Transform inventoryDrag;
    [SerializeField] SOItems taser;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform handSlotsContainer;
    [SerializeField] private Transform equipmentSlotsContainer;
    Transform slotContainer;
    Transform hover;
    Transform divider;
    Transform drag;
    public List<Item> inventory;
    private List<Item> equipments;
    private int nbSlots = 0;
    private Item baseDragItem;
    private bool isInInventory = false;
    private int dividerIndex;
    private int nbHandSlots = 3;
    private int nbEquipmentSlots = 3;

    public Item BaseDragItem { get => baseDragItem; set => baseDragItem = value; }
    public Transform Divider { get => divider; }
    public Transform RightHand { get => rightHand; }
    public int NbHandSlots { get => nbHandSlots; }

    private void Start()
    {
        inventory = new List<Item>();
        equipments = new List<Item>();

        for (int i = 0; i < 50; i++)
            AddInventorySlot();

        for (int i = 0; i < nbEquipmentSlots; i++)
        {
            equipments.Add(null);
        }

        Item newItem = new Item();
        newItem.itemType = taser;
        newItem.stacks = 1;
        inventory[0] = newItem;

        PlayerInputManager.Instance.OpenInventoryAction(OpenInventory, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.CloseInventoryAction(CloseInventory, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.AddItemAction(CheatItem, PlayerInputManager.ActionType.Add);
    }

    #region Inventory

    public void AddInventorySlot()
    {
        inventory.Add(null);
        nbSlots++;
    }

    private void CreateHandSlots()
    {
        for (int i = 0; i < nbHandSlots; i++)
        {
            int index = i;
            GameObject go = Instantiate(equipmentSlotPrefab, handSlotsContainer);
            EquipmentSlot equipmentSlot = go.GetComponent<EquipmentSlot>();
            equipmentSlot.Init(index);
            equipmentSlot.UpdateSlot(Player.Instance.GetToolItem(index));
        }
    }

    private void CreateEquipmentSlots()
    {
        for (int i = 0; i < nbEquipmentSlots; i++)
        {
            int index = i;
            GameObject go = Instantiate(equipmentSlotPrefab, equipmentSlotsContainer);
            EquipmentSlot equipmentSlot = go.GetComponent<EquipmentSlot>();
            equipmentSlot.Init(NbHandSlots + index);
            equipmentSlot.UpdateSlot(equipments[index]);
        }
    }

    public void CreateInventory(Transform _parent, Transform _hover, Transform _divider, Transform _drag)
    {
        for (int i = 0; i < nbSlots; i++)
        {
            int index = i;
            GameObject go = Instantiate(slotPrefab, _parent);
            InventorySlot slot = go.GetComponent<InventorySlot>();
            slot.Init(index);
            slot.UpdateSlot(inventory[index]);
        }

        slotContainer = _parent;
        hover = _hover;
        divider = _divider;
        drag = _drag;
    }

    public void DestroyInventory()
    {
        for (int i = 0; i < slotContainer.childCount; i++)
        {
            Destroy(slotContainer.GetChild(i).gameObject);
        }

        hover.gameObject.SetActive(false);
        HideDivider();
        drag.gameObject.SetActive(false);
        slotContainer = null;
        hover = null;
        divider = null;
        drag = null;
    }

    private void DestroyHandSlots()
    {
        for (int i = 0; i < handSlotsContainer.childCount; i++)
        {
            Destroy(handSlotsContainer.GetChild(i).gameObject);
        }
    }

    private void DestroyEquipmentSlots()
    {
        for (int i = 0; i < equipmentSlotsContainer.childCount; i++)
        {
            Destroy(equipmentSlotsContainer.GetChild(i).gameObject);
        }
    }

    private void OpenInventory()
    {
        if (!isInInventory)
        {
            PlayerInputManager.Instance.DisableCamera();
            PlayerInputManager.Instance.DisableHudMachine();
            PlayerInputManager.Instance.DisableMenuing();
            PlayerInputManager.Instance.DisableBuild();
            PlayerInputManager.Instance.DisableAction();

            PlayerInputManager.Instance.EnableInventory();
            PlayerInputManager.Instance.EnableMovement();

            inventoryCanvas.SetActive(true);
            CreateInventory(inventorySlotContainer, inventoryHover, inventoryDivider, inventoryDrag);
            CreateHandSlots();
            CreateEquipmentSlots();
            Player.Instance.SetIsInMenu(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            isInInventory = true;
        }
    }

    public void CloseInventory()
    {
        if (isInInventory)
        {
            PlayerInputManager.Instance.EnableCamera();
            PlayerInputManager.Instance.EnableHudMachine();
            PlayerInputManager.Instance.EnableMenuing();
            PlayerInputManager.Instance.EnableBuild();
            PlayerInputManager.Instance.EnableAction();
            PlayerInputManager.Instance.EnableMovement();

            PlayerInputManager.Instance.DisableInventory();
            DestroyInventory();
            DestroyHandSlots();
            DestroyEquipmentSlots();

            inventoryCanvas.SetActive(false);
            Player.Instance.SetIsInMenu(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isInInventory = false;
        }
    }
    #endregion

    #region Slot
    public void UpdateInventory(int _index, Item _item)
    {
        inventory[_index] = _item;
    }

    public void UpdateEquipments(int _index, Item _item)
    {
        equipments[_index] = _item;
    }

    public void SetDragSprite(Sprite _sprite)
    {
        if (drag != null)
        {
            drag.GetComponent<Image>().sprite = _sprite;
            drag.gameObject.SetActive(_sprite == null ? false : true);
        }
    }

    public void FollowMouse()
    {
        if (drag != null)
        {
            drag.position = Input.mousePosition;
        }
    }

    public void ShowMouseHover(bool _bool, SOItems _item = null, Vector3 _pos = new Vector3())
    {
        if (hover != null)
        {
            hover.gameObject.SetActive(_bool);
            if (_bool)
            {
                hover.transform.GetChild(1).GetComponent<TMP_Text>().SetText(_item.name);
                hover.transform.GetChild(2).GetComponent<TMP_Text>().SetText(_item.description);
                // Magic number !!!
                hover.transform.position = new Vector3(_pos.x + 175f, _pos.y, _pos.z);
            }
        }
    }

    public void ShowDivider(int _index, Vector3 _pos = new Vector3())
    {
        if (divider != null)
        {
            divider.GetChild(0).GetComponent<Slider>().maxValue = inventory[_index].stacks;
            dividerIndex = _index;
            divider.gameObject.SetActive(true);
            divider.GetChild(5).GetComponent<TextMeshProUGUI>().text = inventory[dividerIndex].stacks.ToString();
            divider.GetChild(0).GetComponent<Slider>().value = inventory[_index].stacks / 2;
            divider.GetChild(6).GetComponent<TextMeshProUGUI>().text = Mathf.Clamp(inventory[_index].stacks / 2, 1f, inventory[_index].stacks).ToString();
            divider.transform.position = new Vector3(_pos.x + 175f, _pos.y, _pos.z);
        }
    }

    public void HideDivider()
    {
        if (divider != null)
        {
            divider.GetChild(0).GetComponent<Slider>().onValueChanged.RemoveAllListeners();
            divider.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            divider.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            divider.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
            divider.gameObject.SetActive(false);
        }
    }

    public void OnSliderValueChange()
    {
        if (divider != null)
        {
            divider.GetChild(6).GetComponent<TextMeshProUGUI>().text = divider.GetChild(0).GetComponent<Slider>().value.ToString();
        }
    }

    public void ConfirmDivide()
    {
        if (divider != null)
        {
            if (slotContainer.GetChild(dividerIndex).GetComponent<InventorySlot>().Item.stacks > 1)
            {
                int index = FindAvailableSlot();
                if (index != -1)
                {
                    int dividedStacks = (int)divider.GetChild(0).GetComponent<Slider>().value;
                    FillNewSlot(index, inventory[dividerIndex].itemType, dividedStacks);
                    inventory[dividerIndex].stacks = inventory[dividerIndex].stacks - dividedStacks;
                    slotContainer.GetChild(dividerIndex).GetComponent<InventorySlot>().UpdateSlot(inventory[dividerIndex]);
                }
            }
            HideDivider();
        }
    }
    #endregion

    #region Update Inventory
    private void FillNewSlot(int _index, SOItems _item, int _stacks)
    {
        Item newItem = new Item();
        newItem.itemType = _item;
        newItem.stacks = _stacks;
        inventory[_index] = newItem;

        if (slotContainer != null)
            slotContainer.GetChild(_index).GetComponent<InventorySlot>().UpdateSlot(inventory[_index]);
    }

    /// <summary>
    /// add the item in the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stacks"></param>
    public void AddItem(SOItems _item, int _stacks)
    {
        int maxStack = 100;
        List<Item> findItem = inventory.Where(items => items != null && items.itemType == _item && items.stacks < maxStack).ToList();
        int stacksToAdd = _stacks;
        int indexFindItem = 0;

        while (stacksToAdd != 0 && indexFindItem != findItem.Count)
        {
            int addNumber = 0;
            if (findItem[indexFindItem].stacks + stacksToAdd <= maxStack)
            {
                addNumber = stacksToAdd;
            }
            else
            {
                addNumber = maxStack - findItem[indexFindItem].stacks;
            }

            findItem[indexFindItem].stacks += addNumber;

            if (slotContainer != null)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] != null && inventory[i].itemType == _item)
                    {
                        slotContainer.GetChild(i).GetComponent<InventorySlot>().UpdateSlot(inventory[i]);
                    }
                }
            }

            stacksToAdd -= addNumber;
            indexFindItem++;
        }

        while (stacksToAdd > 0 && FindAvailableSlot() != -1)
        {
            int index = FindAvailableSlot();

            int min = Mathf.Min(maxStack, stacksToAdd);
            FillNewSlot(index, _item, min);
            stacksToAdd -= min;
        }

        PlayerUi.Instance.UpdateQuantityInInventory();
    }

    private int SortItemsByStacks(Item _item1, Item _item2)
    {
        if (_item1.stacks < _item2.stacks)
        {
            return -1;
        }
        else if (_item1.stacks > _item2.stacks)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// remove the item in the inventory, return the success of the operation
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stacks"></param>
    /// <returns></returns>
    public bool RemoveItem(SOItems _item, int _stacks)
    {
        if (GetItemAmount(_item) >= _stacks)
        {
            int stacksToSubtract = _stacks;
            int findItemIndex = 0;

            List<Item> findItem = inventory.Where(items => items != null && items.itemType == _item).ToList();
            findItem.Sort(SortItemsByStacks);

            while (stacksToSubtract > 0)
            {
                int stacksSubtracted = (int)Mathf.Min(findItem[findItemIndex].stacks, stacksToSubtract);
                findItem[findItemIndex].stacks -= stacksSubtracted;

                if (slotContainer != null)
                {
                    for (int i = 0; i < inventory.Count; i++)
                    {
                        if (inventory[i] != null && inventory[i].itemType == _item)
                        {
                            slotContainer.GetChild(i).GetComponent<InventorySlot>().UpdateSlot(inventory[i]);
                            if (inventory[i].stacks <= 0)
                            {
                                inventory[i] = null;
                            }
                        }
                    }
                }

                stacksToSubtract -= stacksSubtracted;
                findItemIndex++;
            }
            PlayerUi.Instance.UpdateQuantityInInventory();
            return true;
        }
        return false;
    }

    public void RemoveItemAtIndex(int _index)
    {
        inventory[_index] = null;
        slotContainer.GetChild(_index).GetComponent<InventorySlot>().UpdateSlot(inventory[_index]);
        PlayerUi.Instance.UpdateQuantityInInventory();
    }

    private int SortInventoryByTypeAndStacks(Item _item1, Item _item2)
    {
        if (_item1 != null && _item2 != null)
        {
            if (String.Compare(_item1.itemType.NameItem, _item2.itemType.NameItem, true) == -1)
            {
                return -1;
            }
            else if (String.Compare(_item1.itemType.NameItem, _item2.itemType.NameItem, true) == 0 && _item1.stacks > _item2.stacks)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        if (_item1 == null && _item2 != null)
        {
            return 1;
        }
        if (_item1 != null && _item2 == null)
        {
            return -1;
        }
        return 0;
    }

    private void PackItems()
    {
        int maxStack = 100;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null && inventory[i].stacks != maxStack)
            {
                for (int j = 0; j < inventory.Count; j++)
                {
                    if (inventory[j] != null && i != j && inventory[i].itemType == inventory[j].itemType && inventory[j].stacks != maxStack && inventory[i].stacks > 0)
                    {
                        int stacksAvailable = maxStack - inventory[j].stacks;
                        int stacksAdded = Mathf.Min(inventory[i].stacks, stacksAvailable);
                        inventory[j].stacks += stacksAdded;
                        inventory[i].stacks -= stacksAdded;
                    }
                }

                if (inventory[i].stacks == 0)
                {
                    inventory[i] = null;
                }
            }
        }
    }

    public void SortInventory()
    {
        PackItems();
        inventory.Sort(SortInventoryByTypeAndStacks);
        for (int i = 0; i < inventory.Count; i++)
        {
            slotContainer.GetChild(i).GetComponent<InventorySlot>().UpdateSlot(inventory[i]);
        }
    }
    #endregion

    #region Get Inventory
    private int FindAvailableSlot()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// return the amount of the item in the invenotry
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    public int GetItemAmount(SOItems _item)
    {
        List<Item> findItem = inventory.Where(items => items != null && items.itemType == _item)?.ToList();

        int stacks = 0;
        if (findItem != null)
        {
            for (int i = 0; i < findItem.Count; i++)
            {
                stacks += findItem[i].stacks;
            }
        }

        return stacks;
    }

    /// <summary>
    /// return if the item can be put in the inventory
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    public bool CanAddItem(SOItems item)
    {
        return inventory.Where(slot => slot == null || (slot.itemType == item && slot.stacks < 100)).ToList().Count > 0 ? true : false;
    }

    /// <summary>
    /// return if any slot are empty
    /// </summary>
    /// <returns></returns>
    public bool CanAddItem()
    {
        return inventory.Where(slot => slot == null).ToList().Count > 0 ? true : false;
    }

    public bool IsInventoryOpen()
    {
        return inventoryCanvas.activeInHierarchy;
    }

    /// <summary>
    /// return the number of stacks that can put into the inventory consider empty slot and other
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetStackAvailable(SOItems item)
    {
        List<Item> stacks = inventory.Where(slot => slot == null || (slot.itemType == item && slot.stacks < 100)).ToList();
        int stacksAvailable = 0;
        for (int i = 0; i < stacks.Count; i++)
        {
            if (stacks[i] == null)
            {
                stacksAvailable += 100;
            }
            else
            {
                stacksAvailable += (100 - stacks[i].stacks);
            }
        }

        return stacksAvailable;
    }

    public List<Item> GetHandSlotsItems()
    {
        List<Item> items = new List<Item>();

        for (int i = 0; i < nbHandSlots; i++)
        {
            int index = i;
            items.Add(Player.Instance.GetToolItem(index));
        }

        return items;
    }

    public void SetHandSlotsItems(List<Item> items)
    {
        for (int i = 0; i < nbHandSlots; i++)
        {
            int index = i;
            if (items[index] != null)
            {
                GameObject go = Instantiate(items[i].itemType.gameObject, rightHand.GetChild(index));
                Player.Instance.AddHandItem(index, items[index], go);
            }
        }
    }

    #endregion

    #region Cheat
    /// <summary>
    /// clear the inventory (erase all the item)
    /// </summary>
    public void CleanInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].itemType = null;
            inventory[i].stacks = 0;
            inventory[i] = null;
        }
    }

    private void CheatItem()
    {
        for (int i = 0; i < database.AllItems.Count; i++)
        {
            AddItem(database.AllItems[i], 100);
        }
    }
    #endregion
}