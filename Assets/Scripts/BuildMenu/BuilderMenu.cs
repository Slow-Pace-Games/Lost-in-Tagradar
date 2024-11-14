using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuilderMenu : MonoBehaviour
{
    [Header("Middle panel variants")]
    [SerializeField] GameObject specialPanel;
    [SerializeField] GameObject productionPanel;
    [SerializeField] GameObject powerPanel;
    [SerializeField] GameObject logisticsPanel;
    [SerializeField] GameObject organizationPanel;
    [SerializeField] GameObject searchPanel;

    [Header("Search bar")]
    [SerializeField] GameObject searchBar;

    public GameObject currentPanel;

    private void Start()
    {
        InitPanels();
    }

    public void InitPanels()
    {
        currentPanel = specialPanel;
        currentPanel.GetComponent<IEditablePanel>().InitPanel();
        specialPanel.SetActive(true);
        productionPanel.SetActive(false);
        powerPanel.SetActive(false);
        logisticsPanel.SetActive(false);
        organizationPanel.SetActive(false);
    }

    public void ActivePanel(GameObject panel)
    {
        currentPanel.GetComponent<IEditablePanel>().ResetPanel();
        currentPanel.SetActive(false);
        panel.SetActive(true);
        currentPanel = panel;
        currentPanel.GetComponent<IEditablePanel>().InitPanel();
    }

    public void ActiveSearchPanel(List<SOBuildingData> buildings)
    {
        if (currentPanel != searchPanel)
        {
            currentPanel.GetComponent<IEditablePanel>().ResetPanel();
            currentPanel.SetActive(false);
            searchPanel.SetActive(true);
            currentPanel = searchPanel;
        }

        currentPanel.GetComponent<SearchPanel>().InitPanel(buildings);
    }
    public void SearchInBuildings()
    {
        string enteredText = searchBar.GetComponent<TMP_InputField>().text;
        List<SOBuildingData> buildings = BuildMenu.Instance.Database.AllBuildingData.Where(recipe => recipe.name.ToLower().Contains(enteredText)).ToList();
        ActiveSearchPanel(buildings);
    }

    public void DeactiveMenusKeys()
    {
        PlayerInputManager.Instance.DisableBuildMenu();
    }

    public void ReactiveMenusKeys()
    {
        PlayerInputManager.Instance.EnableBuildMenu();
    }
}
