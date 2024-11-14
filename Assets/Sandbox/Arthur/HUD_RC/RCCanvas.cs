using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RCCanvas : MonoBehaviour
{
    [Header("Canvas Component")]
    [SerializeField] Transform containerPanel;
    [SerializeField] Transform inventoryPanel;
    [SerializeField] Transform hover;
    [SerializeField] Transform divider;
    [SerializeField] Transform drag;
    [SerializeField] Transform[] tabs;
    [SerializeField] Button[] buttonTabs;

    [Header("Manager")]
    [SerializeField] private ResearchTree researchTree;
    [SerializeField] private CraftBench craftBench;

    private void Start()
    {
        researchTree.InitResearchTree();
    }

    private void Update()
    {
        if (researchTree.isResearching)
        {
            researchTree.UpdateResearchDuration();
        }
    }

    public void OpenRCCanvas()
    {
        containerPanel.gameObject.SetActive(true);
        Player.Instance.CreateInventory(inventoryPanel, hover, divider, drag);
    }

    public void CloseRCCanvas()
    {
        containerPanel.gameObject.SetActive(false);
        Player.Instance.DestroyInventory(); 
        PlayerInputManager.Instance.DisableCrafting();
    }

    public void CloseRCCanvasWithCross()
    {
        Player.Instance.OpenCloseBuildingMenu();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChangeTab(int id)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == id)
            {
                tabs[i].gameObject.SetActive(true);
                buttonTabs[i].interactable = false;
            }
            else
            {
                tabs[i].gameObject.SetActive(false);
                buttonTabs[i].interactable = true;
            }
        }

        if (id == 1)
        {
            craftBench.InitCraftBenchRC();
            craftBench.OpenCraftBenchRC();
        }
    }
}
