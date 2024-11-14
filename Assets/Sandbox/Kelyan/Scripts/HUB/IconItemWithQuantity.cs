using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconItemWithQuantity : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI quantity;
    private string quantityInventory;
    private string quantityNeeded;
    private int quantityNeededInt;
    private SOItems item = null;
    public SOItems Item { get => item; }

    public void Init(Sprite icon, int quantityNeeded, int quantityInInventory)
    {
        image.sprite = icon;
        quantityInventory = quantityInInventory.ToString();
        this.quantityNeeded = quantityNeeded.ToString();
        quantityNeededInt = quantityNeeded;
        if (quantityInInventory < quantityNeeded)
        {
            quantity.color = Color.red;
        }
        else
        {
            quantity.color = Color.green;
        }
        quantity.text = quantityInventory + "/" + this.quantityNeeded;

    }    
    public void Init(int quantityNeeded, int quantityInInventory,SOItems itemToSet)
    {
        item = itemToSet;
        image.sprite = item.Sprite;
        quantityInventory = quantityInInventory.ToString();
        this.quantityNeeded = quantityNeeded.ToString();
        quantityNeededInt = quantityNeeded;
        if (quantityInInventory < quantityNeeded)
        {
            quantity.color = Color.red;
        }
        else
        {
            quantity.color = Color.green;
        }
        quantity.text = quantityInventory + "/" + this.quantityNeeded;
    }
    public void Init(Sprite icon, int quantity)
    {
        image.sprite = icon;
        this.quantity.text = quantity.ToString();
    }

    public void UpdateQuantityInventory(int quantityInInventory)
    {
        if (quantityInInventory < quantityNeededInt)
        {
            quantity.color = Color.red;
        }
        else
        {
            quantity.color = Color.green;
        }
        quantityInventory = quantityInInventory.ToString();
        quantity.text = quantityInventory + "/" + quantityNeeded;
    }

    public void UpdateQuantity(int quantity)
    {
        this.quantity.color = Color.green;
        this.quantity.text = quantity.ToString();
    }

    public void UpdateQuantity(int quantityNeeded, int quantityInInventory)
    {
        if(quantityInInventory < quantityNeeded) 
        {
            quantity.color = Color.red;
        }
        else
        {
            quantity.color = Color.green;
        }
        quantity.text = quantityInInventory.ToString() + "/" + quantityNeeded.ToString();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
