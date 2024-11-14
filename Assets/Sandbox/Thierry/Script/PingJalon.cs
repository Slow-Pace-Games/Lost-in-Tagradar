using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PingJalon : MonoBehaviour
{
    [SerializeField] public List<IconItemWithQuantity> items;
    [SerializeField] public TextMeshProUGUI title;

    public void UpdateQuantityInInventory()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Item != null)
            {
                items[i].UpdateQuantityInventory(Player.Instance.GetItemAmount(items[i].Item));
            }
        }
    }

}
