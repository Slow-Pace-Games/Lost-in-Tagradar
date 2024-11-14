using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chest : Building, IMachineInteractable, ISelectable
{
    [SerializeField] protected GameObject chestSlotPrefab;
    [SerializeField] Transform[] outliner;
    protected Transform chestSlotContainer;
    protected Transform divider;

    public List<Item> inventory;
    protected int nbSlots;
    protected int dividerIndex;

    protected bool isWindowOpen = false;

    public Transform Divider { get => divider; }

    [Header("Selection")]
    [SerializeField] private string type;
    public string Type { get { return type; } }

    private RessourcesRayManager ressourcesRayManager = RessourcesRayManager.instance;

    public void InitChest()
    {
        if (inventory.Count == 0)
        {
            inventory = new List<Item>();
            nbSlots = ChestCanvas.Instance.NbSlots;
            chestSlotContainer = ChestCanvas.Instance.ChestSlotContainer;
            for (int i = 0; i < nbSlots; i++)
            {
                inventory.Add(null);
            }
        }
    }

    private void Start()
    {
        InitChest();
    }

    private void Update()
    {
        if (IsPlaced)
        {
            if (instantiatedEffect != null)
            {
                effectTimer += Time.deltaTime;
                if (effectTimer >= effectMaxTimer)
                {
                    Destroy(instantiatedEffect.gameObject);
                    ActivateMesh(true);
                }
            }
        }
    }

    #region Inventory

    private void CreateChestInventory()
    {
        for (int i = 0; i < nbSlots; i++)
        {
            int index = i;
            GameObject go = Instantiate(chestSlotPrefab, chestSlotContainer);
            ChestSlot chestSlot = go.GetComponent<ChestSlot>();
            chestSlot.Init(index, this);
            chestSlot.UpdateSlot(inventory[index]);
        }
        divider = ChestCanvas.Instance.Divider;
    }

    private void DestroyChestInventory()
    {
        for (int i = 0; i < chestSlotContainer.childCount; i++)
        {
            Destroy(chestSlotContainer.GetChild(i).gameObject);
        }
        divider = null;
    }

    public void OpenWindow()
    {
        ChestCanvas.Instance.OpenCanvas();
        CreateChestInventory();
        isWindowOpen = true;
    }

    public void CloseWindow()
    {
        ChestCanvas.Instance.CloseCanvas();
        DestroyChestInventory();
        isWindowOpen = false;
    }

    #endregion

    #region Slot
    public void UpdateInventory(int _index, Item _item)
    {
        inventory[_index] = _item;
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
            if (chestSlotContainer.GetChild(dividerIndex).GetComponent<ChestSlot>().Item.stacks > 1)
            {
                int index = FindAvailableSlot();
                if (index != -1)
                {
                    int dividedStacks = (int)divider.GetChild(0).GetComponent<Slider>().value;
                    FillNewSlot(index, inventory[dividerIndex].itemType, dividedStacks);
                    inventory[dividerIndex].stacks = inventory[dividerIndex].stacks - dividedStacks;
                    chestSlotContainer.GetChild(dividerIndex).GetComponent<ChestSlot>().UpdateSlot(inventory[dividerIndex]);
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

        if (chestSlotContainer != null)
            chestSlotContainer.GetChild(_index).GetComponent<ChestSlot>().UpdateSlot(inventory[_index]);
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

    public void RemoveItemAtIndex(int _index)
    {
        inventory[_index] = null;
        chestSlotContainer.GetChild(_index).GetComponent<ChestSlot>().UpdateSlot(inventory[_index]);
    }

    #endregion

    #region ISelectable

    public void Select()
    {
        for (int i = 0; i < outliner.Length; i++)
        {
            outliner[i].gameObject.layer = 8;
        }
    }

    public void Deselect()
    {
        if (isPlaced)
        {
            for (int i = 0; i < outliner.Length; i++)
            {
                outliner[i].gameObject.layer = 0;
            }
        }
    }

    public void Interact()
    {
        if (IsPlaced)
        {
            if (ressourcesRayManager.LastSelected != null && this != ressourcesRayManager.LastSelected as Chest)
            {
                ressourcesRayManager.LastSelected.Deselect();
            }

            ressourcesRayManager.LastSelected = gameObject.GetComponent<ISelectable>();
            Select();

            if (!Player.Instance.GetOnDestructionMode())
            {
                ressourcesRayManager.InteractionMessage.SetMachineTextEnable(Type);
            }
            else
            {
                ressourcesRayManager.InteractionMessage.SetDestructTextEnable(Type);
            }
        }
    }

    #endregion

    public List<Item> GetItemsInChest()
    {
        List<Item> ids = new List<Item>();
        foreach (Item item in inventory)
        {
            ids.Add(item);
        }

        return ids;
    }

    public void SetItemsInChest(List<Item> itemChestIds)
    {
        for (int i = 0; i < itemChestIds.Count; ++i)
        {
            if (itemChestIds[i] != null)
            {
                inventory[i] = new Item();
                inventory[i].itemType = itemChestIds[i].itemType;
                inventory[i].stacks = itemChestIds[i].stacks;
            }
        }
    }
}
