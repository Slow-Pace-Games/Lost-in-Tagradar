using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ItemHUD : MonoBehaviour
{
    [SerializeField] GameObject inputOutputSlot;
    public int stack = 0;
    private SOItems currentItem;
    private List<SOItems> itemsAccepted;
    [SerializeField] TextMeshProUGUI textQuantityName;
    [SerializeField] TextMeshProUGUI textQuantityPerMinute;
    public SOItems GetItemType()
    {
        return currentItem;
    }

    public void Init(int valueStack, float meltTime, SOItems currentItem, List<SOItems> itemsAccepted = null)
    {
        this.currentItem = currentItem;
        this.itemsAccepted = itemsAccepted;

        inputOutputSlot.GetComponent<InputOutputSlot>().Init(currentItem, itemsAccepted);

        textQuantityName.text = valueStack.ToString() + " " + currentItem.NameItem;
        float quantityPerMinute = Mathf.Round((valueStack * 60) / meltTime);
        textQuantityPerMinute.text = quantityPerMinute.ToString() + " per minute";
    }

    public void UpdateUI(int valueStack, float meltTime, SOItems currentItem) 
    {
        this.currentItem = currentItem;
        textQuantityName.text = valueStack.ToString() + " " + currentItem.NameItem;
        float quantityPerMinute = Mathf.Round((valueStack * 60) / meltTime);
        textQuantityPerMinute.text = quantityPerMinute.ToString() + " per minute";
    }
    public void SetStackValue(int stackValue)
    {
        stack = stackValue;
        inputOutputSlot.GetComponent<InputOutputSlot>().UpdateSlot(stack);
    }
}
