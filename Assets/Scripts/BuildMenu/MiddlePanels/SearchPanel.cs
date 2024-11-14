using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchPanel : BuildMenuMiddlePanel, IEditablePanel
{
    public void InitPanel(List<SOBuildingData> buildings)
    {
        ResetPanel();

        // Create a machineCategory Prefab with name categoryName
        GameObject newMachineCategory = Instantiate(BuildMenu.Instance.MachineCategoryPrefab, gameObject.transform);
        newMachineCategory.GetComponent<MachineCategoryContainer>().categoryName.text = "1. Search result";

        // Clear buttonListInfo
        buttonInfo.Clear();

        // Add machineContainer Prefabs of finding buildings
        if (buildings.Count > 0)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].isDiscovered)
                {
                    Transform machinesContainer = newMachineCategory.GetComponent<MachineCategoryContainer>().machinesContainer.transform;
                    GameObject newBuilding = Instantiate(BuildMenu.Instance.MachineContainerPrefab, machinesContainer);
                    int index = i;
                    InitBuildingButton(newBuilding, buildings[i]);

                    // Add function on building button to enter in building mode with this building chosen
                    newBuilding.GetComponentInChildren<Button>().onClick.AddListener(() => BuildMenu.Instance.OpenOrCloseBuilderMenu());

                    // TODO Mettre au SOBuildingData le PrefabsHotBar.PrefabsType selon building ou convey
                    newBuilding.GetComponentInChildren<Button>().onClick.AddListener(() => Player.Instance.PlayerBuildance.PlaceBuildingWithoutHotbar(buildings[index].prefab, buildings[index].prefabType));
                }
            }
        }
    }
}
