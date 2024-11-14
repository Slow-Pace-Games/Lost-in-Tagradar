using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRecipeHUB : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextItemName;
    [SerializeField] TextMeshProUGUI TextQuantityProductable;
    [SerializeField] Image icon;
    public void Init(SOItems itemType)
    {
        TextItemName.text = itemType.NameItem;
        icon.sprite = itemType.Sprite;
    }

    public void UpdateQuantityProductable(int quantityProductable)
    {
        TextQuantityProductable.text = "[ " + quantityProductable.ToString() + " ]";
    }

}
