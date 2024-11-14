using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCheat : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SODatabase database;
    public delegate void ItemClickEvent(int index);
    public ItemClickEvent onItemClick;

    [Header("Inventory")]
    [SerializeField] private Button clean;
    [SerializeField] private Button increaseNbSlot;

    [Header("Item")]
    [SerializeField] private GameObject prefabCheatItem;
    [SerializeField] private RectTransform containerCheatButton;
    [SerializeField] private Slider valueItem;
    [SerializeField] private TextMeshProUGUI valueItemToGive;
    [SerializeField] private Button giveItem;
    private List<ItemCheat> allItems = new List<ItemCheat>();
    private SOItems currentItem = null;
    private int spaceInInventory = 10;

    private void Start()
    {
        onItemClick += OnItemClick;
        giveItem.onClick.AddListener(AddItem);
        clean.onClick.AddListener(Player.Instance.CleanInventory);
        increaseNbSlot.onClick.AddListener(Player.Instance.AddInventorySlot);
        valueItem.onValueChanged.AddListener((value) => valueItemToGive.text = value.ToString("F1"));
        valueItem.maxValue = 0;
        valueItem.value = 0;

        InitAllItem();
    }

    private void InitAllItem()
    {
        for (int i = 0; i < database.AllItems.Count; i++)
        {
            GameObject itemCheat = Instantiate(prefabCheatItem, containerCheatButton);
            ItemCheat cheatitem = itemCheat.GetComponent<ItemCheat>();
            allItems.Add(cheatitem);

            SOItems item = database.AllItems[i];
            allItems[i].Init(item.Sprite, item.NameItem, onItemClick);
        }
    }

    private void OnItemClick(int index)
    {
        if (currentItem != null)
            allItems.Where(item => item.Selected == true && item.transform.GetSiblingIndex() != index).Select(item => item).First().Selected = false;
        currentItem = database.AllItems[index];

        int spaceInventory = Player.Instance.GetStackAvailable(currentItem);
        valueItem.maxValue = Mathf.Clamp(spaceInventory, 0, 100);
        valueItem.value = 0;
    }

    private void AddItem()
    {
        if (currentItem != null && spaceInInventory != 0)
        {
            spaceInInventory -= (int)valueItem.value;
            Player.Instance.AddItem(currentItem, (int)valueItem.value);

            if (spaceInInventory <= 0)
            {
                spaceInInventory = 10;
                valueItem.value = 100;
                return;
            }

            if (spaceInInventory <= 100)
            {
                valueItem.maxValue = spaceInInventory;
            }
        }
    }
}