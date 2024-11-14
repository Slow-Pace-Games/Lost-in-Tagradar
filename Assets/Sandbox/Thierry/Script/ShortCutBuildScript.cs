using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShortCutBuildScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameMachine;
    [SerializeField] List<IconItemWithQuantity> icons;
    List<BuildingCost> costList = new List<BuildingCost>();

    public void  UpdateShortCutBuild(SOBuildingData building)
    {
        if (building == null) 
            return;
        nameMachine.text = building.name;
        UpdateCost(building);
    }

    private void UpdateCost(SOBuildingData building)
    {
        costList = new List<BuildingCost>();
        if (building != null)
        {
            costList = building.costs;
        }
        if (costList != null)
        {
            for (int i = 0; i < costList.Count; i++)
            {
                icons[i].Init(costList[i].value, Player.Instance.GetItemAmount(costList[i].item), costList[i].item);
                icons[i].Enable();
            }

            for (int i = costList.Count; i < icons.Count; i++)
            {
                icons[i].Disable();
            }
        }
        else
        {
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].Disable();
            }
        }
    }

    public void UpdateCost()
    {
        if (costList != null)
        {
            for (int i = 0; i < costList.Count; i++)
            {
                icons[i].Init(costList[i].value, Player.Instance.GetItemAmount(costList[i].item), costList[i].item);
                icons[i].Enable();
            }

            for (int i = costList.Count; i < icons.Count; i++)
            {
                icons[i].Disable();
            }
        }
        else
        {
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].Disable();
            }
        }
    }
}
