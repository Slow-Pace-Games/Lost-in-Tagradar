using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region Class ItemNeeded
[System.Serializable]
class ItemNeededToRepair
{
    public SOItems item;
    public int quantity;
}
#endregion
public class RepairMenu : MonoBehaviour
{
    [Header("Items Needed To Repair")]
    [SerializeField] private List<ItemNeededToRepair> itemsNeededToRepair;

    [Header("Slots Items")]
    [SerializeField] private List<IconItemWithQuantity> itemsSlots;

    [Header("Images Link")]
    [SerializeField] private List<Image> imagesLink;

    [Header("Repair Button")]
    [SerializeField] private Button repairButton;
    [SerializeField] private Image backgroundButton;

    public bool isRepaired = false;
    public void Init()
    {
        for (int i = 0; i < itemsSlots.Count; i++)
        {
            int quantityInInventory = Player.Instance.GetItemAmount(itemsNeededToRepair[i].item);
            itemsSlots[i].Init(itemsNeededToRepair[i].item.Sprite, itemsNeededToRepair[i].quantity, quantityInInventory);
        }
        repairButton.onClick.AddListener(() => Repair());
    }

    private void UpdateItemsSlots()
    {
        for (int i = 0; i < itemsSlots.Count; i++)
        {
            int quantityInInventory = Player.Instance.GetItemAmount(itemsNeededToRepair[i].item);
            itemsSlots[i].UpdateQuantityInventory(quantityInInventory);
        }
    }
    private bool AreItemsNeededComplete()
    {
        for (int i = 0; i < itemsNeededToRepair.Count; i++)
        {
            int quantityInInventory = Player.Instance.GetItemAmount(itemsNeededToRepair[i].item);
            if (quantityInInventory < itemsNeededToRepair[i].quantity)
            {
                return false;
            }
        }

        return true;
    }

    public void Repair(bool haveTopay = true)
    {
        if (haveTopay)
        {
            if (AreItemsNeededComplete() && !isRepaired)
            {
                for (int i = 0; i < itemsNeededToRepair.Count; i++)
                {
                    Player.Instance.RemoveItem(itemsNeededToRepair[i].item, itemsNeededToRepair[i].quantity);
                }
                isRepaired = true;
                repairButton.interactable = false;
                SpaceshipCanvas.Instance.onNbPartsRepairedIncrease?.Invoke();
            }
        }
        else
        {
            isRepaired = true;
            repairButton.interactable = false;
            SpaceshipCanvas.Instance.onNbPartsRepairedIncrease?.Invoke();
        }
    }
    public void OpenRepairMenu()
    {
        gameObject.SetActive(true);
        HideHudWhenCameraMove();
        UpdateItemsSlots();
    }

    private void HideHudWhenCameraMove()
    {
        for (int i = 0; i < itemsSlots.Count; i++)
        {
            itemsSlots[i].gameObject.SetActive(false);
            imagesLink[i].gameObject.SetActive(false);
        }
        repairButton.gameObject.SetActive(false);
        backgroundButton.gameObject.SetActive(false);

        StartCoroutine(WaitCameraStopMoving());
    }

    IEnumerator WaitCameraStopMoving()
    {
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < itemsSlots.Count; i++)
        {
            itemsSlots[i].gameObject.SetActive(true);
            imagesLink[i].gameObject.SetActive(true);
        }
        repairButton.gameObject.SetActive(true);
        backgroundButton.gameObject.SetActive(true);
    }
}
