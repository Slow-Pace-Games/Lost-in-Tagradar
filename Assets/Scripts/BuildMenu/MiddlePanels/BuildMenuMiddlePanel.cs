using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuMiddlePanel : MonoBehaviour
{
    protected List<DisplayBuildingInfos> buttonInfo = new List<DisplayBuildingInfos>();

    private void Start()
    {
        // TODO : Modifier la fonction appelée lors de l'utilisation de la touche
        PlayerInputManager.Instance.HotBarBinding1(() => AssignToHotbar(0), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding2(() => AssignToHotbar(1), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding3(() => AssignToHotbar(2), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding4(() => AssignToHotbar(3), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding5(() => AssignToHotbar(4), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding6(() => AssignToHotbar(5), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding7(() => AssignToHotbar(6), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding8(() => AssignToHotbar(7), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding9(() => AssignToHotbar(8), PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.HotBarBinding10(() => AssignToHotbar(9), PlayerInputManager.ActionType.Add);
    }

    private void AssignToHotbar(int index)
    {
        DisplayBuildingInfos currentBuilding = buttonInfo.Where(building => building.IshHover).FirstOrDefault();
        if (currentBuilding)
        {
            Player.Instance.PlayerBuildance.ChangeBuildingInHotbar(index, currentBuilding.PrefabType, currentBuilding.Prefab, currentBuilding.Icon);
        }
    }

    public void InitMachinesContainer(string categoryName, MachineClass machineClass)
    {
        // Create a machineCategory Prefab with name categoryName
        GameObject newMachineCategory = Instantiate(BuildMenu.Instance.MachineCategoryPrefab, gameObject.transform);
        newMachineCategory.GetComponent<MachineCategoryContainer>().categoryName.text = "1. " + categoryName;

        // Search Building with class "machineClass"
        List<SOBuildingData> buildings = BuildMenu.Instance.Database.AllBuildingData.Where(building => building.machineClass == machineClass).ToList();

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

    protected void InitBuildingButton(GameObject newBuilding, SOBuildingData buildingData)
    {
        // Init sprite and name of building in middle pannel
        newBuilding.GetComponent<BuildMenuMachineContainer>().Init(buildingData);

        // Init sprite, name and description of building for right pannel if mouse hoover building button in middle panel
        newBuilding.GetComponentInChildren<DisplayBuildingInfos>().Init(buildingData);

        buttonInfo.Add(newBuilding.GetComponentInChildren<DisplayBuildingInfos>());
    }
    public void ResetPanel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
