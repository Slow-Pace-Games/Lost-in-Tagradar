using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestSlot : Slot
{
    private Chest chest;
    private int index;

    public void Init(int _i, Chest _chest)
    {
        stacksText = GetComponentInChildren<TextMeshProUGUI>();
        index = _i;
        chest = _chest;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        chest.UpdateInventory(index, item);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!isDroppingOnSameSpot)
        {
            item = Player.Instance.GetBaseDragItem();
        }
        chest.UpdateInventory(index, item);
        Player.Instance.SetDragSprite(null);
        UpdateUI();
        isDroppingOnSameSpot = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                chest.HideDivider();
                chest.ShowDivider(index, transform.position);
                chest.Divider.GetChild(0).GetComponent<Slider>().onValueChanged.AddListener(delegate { chest.OnSliderValueChange(); });
                chest.Divider.GetChild(1).GetComponent<Button>().onClick.AddListener(() => chest.ConfirmDivide());
                chest.Divider.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ItemManager.Instance.DropItem(item.itemType, item.stacks, new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y - Player.Instance.GetComponent<CharacterController>().height / 2f, Player.Instance.transform.position.z + 1f), Quaternion.identity));
                chest.Divider.GetChild(2).GetComponent<Button>().onClick.AddListener(() => chest.HideDivider());
                chest.Divider.GetChild(2).GetComponent<Button>().onClick.AddListener(() => chest.RemoveItemAtIndex(index));
                chest.Divider.GetChild(3).GetComponent<Button>().onClick.AddListener(() => chest.HideDivider());
            }
        }
    }
}