using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuMachineContainer : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI machineName;

    public void Init(SOBuildingData buildingData)
    {
        icon.sprite = buildingData.icon;
        machineName.text = buildingData.name;
    }
}
