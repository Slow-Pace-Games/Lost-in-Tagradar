using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayBuildingInfos : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Sprite icon;
    private string buidingName;
    private string buidingDescription;
    public List<BuildingCost> costsList;
    private bool isHover = false;
    private GameObject prefab;
    private PrefabsType prefabType;

    public bool IshHover { get => isHover; }
    public Sprite Icon { get => icon; }
    public GameObject Prefab { get => prefab; }
    public PrefabsType PrefabType { get => prefabType; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InitPanels();
        isHover = true;
    }

    // Use only if necessary, if not, delete this fonction and the interface IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }

    private void InitPanels()
    {
        InitInfosPanel();
        InitCostPanel();
    }

    private void InitInfosPanel()
    {
        // Active info panel if desactivate
        if (!BuildMenu.Instance.rightPanelInfos.activeSelf)
        {
            BuildMenu.Instance.rightPanelInfos.SetActive(true);
        }

        RightPanelInfosContainer panel = BuildMenu.Instance.rightPanelInfos.GetComponent<RightPanelInfosContainer>();
        // Change building name
        panel.nameBuilding.text = buidingName;
        // Change building icon
        panel.icon.sprite = icon;
        // Change building description
        panel.descriptionBuilding.text = buidingDescription;
    }

    private void InitCostPanel()
    {
        // Active cost panel if desactivate
        if (!BuildMenu.Instance.rightPanelCost.activeSelf)
        {
            BuildMenu.Instance.rightPanelCost.SetActive(true);
        }

        GameObject panel = BuildMenu.Instance.rightIngredientsContainer;
        ResetObject(panel.transform);

        // Add items icon in the horizontal layout, refresh amount of items possessed and needed, and add tooltip on hoover
        for (int i = 0; i < costsList.Count; i++)
        {
            GameObject newIcon = Instantiate(BuildMenu.Instance.ItemCostIcon, panel.transform);
            int index = i;
            ItemCostIcon costIcon = newIcon.GetComponent<ItemCostIcon>();
            costIcon.icon.sprite = costsList[index].item.Sprite;
            costIcon.amountText.text = Player.Instance.GetItemAmount(costsList[index].item) + " / " + costsList[index].value.ToString();
            costIcon.button.onClick.AddListener(() => BuildMenu.Instance.CloseBuilderMenu());

            // Config button for switching to ingredient page in codex
            costIcon.button.onClick.AddListener(() => Codex.Instance.OpenCodex());
            costIcon.button.onClick.AddListener(() => Codex.Instance.CodexMenu.AddPartsToRecipes());
            costIcon.button.onClick.AddListener(() => Codex.Instance.CodexMenu.ChangePartDescription(costsList[index].item));
        }
    }

    private void ResetObject(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void Init(SOBuildingData building)
    {
        this.icon = building.icon;
        buidingName = building.name;
        buidingDescription = building.description;
        costsList = new List<BuildingCost>(building.costs);
        prefab = building.prefab;
        prefabType = building.prefabType;
    }

    //private void AddNewGameObjectWithText()
    //{
    //    GameObject obj = new GameObject();
    //    obj.transform.parent = BuildMenu.instance.rightPanelInfos.GetComponent<LayoutGroup>().transform;
    //    obj.AddComponent<TextMeshProUGUI>();
    //    obj.GetComponent<TextMeshProUGUI>().text = "Super Texte";
    //}   
}
