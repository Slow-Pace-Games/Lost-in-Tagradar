using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMenu : MonoBehaviour
{
    private static BuildMenu instance;
    public static BuildMenu Instance { get => instance; }

    [SerializeField] GameObject canvas;

    [Header("Prefabs")]
    [SerializeField] GameObject machineContainerPrefab;
    [SerializeField] GameObject machineCategoryPrefab;
    [SerializeField] GameObject machineFunctionPrefab;
    [SerializeField] GameObject itemCostIcon;
    [SerializeField] GameObject crossAir;

    [Header("Database")]
    [SerializeField] SODatabase database;

    [Header("Panels")]
    public GameObject rightPanelInfos;
    public GameObject rightPanelCost;
    public GameObject rightIngredientsContainer;

    [Header("Other")]
    [SerializeField] BuilderMenu builderMenu;

    public GameObject MachineContainerPrefab { get { return machineContainerPrefab; } }
    public GameObject MachineCategoryPrefab { get { return machineCategoryPrefab; } }
    public GameObject MachineFunctionPrefab { get { return machineFunctionPrefab; } }
    public GameObject ItemCostIcon { get { return itemCostIcon; } }
    public SODatabase Database { get { return database; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerInputManager.Instance.CloseBuildMenu(CloseBuilderMenu, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.BuildMenuAction(OpenBuilderMenu, PlayerInputManager.ActionType.Add);
    }

    public void OpenOrCloseBuilderMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (!canvas.activeSelf)
        {
            OpenBuilderMenu();
        }
        else
        {
            CloseBuilderMenu();
        }
    }
    private void OpenBuilderMenu()
    {
        crossAir.SetActive(false);
        Player.Instance.SetIsInMenu(true);

        if (builderMenu.currentPanel)
        {
            builderMenu.currentPanel.GetComponent<IEditablePanel>().ResetPanel();
            builderMenu.currentPanel.GetComponent<IEditablePanel>().InitPanel();
        }

        canvas.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        PlayerInputManager.Instance.DisableMovement();
        PlayerInputManager.Instance.DisableHotbarAndBuild();
        PlayerInputManager.Instance.DisableMenuing();
        PlayerInputManager.Instance.DisableCamera();
        PlayerInputManager.Instance.DisableDebugKeys();
        PlayerInputManager.Instance.DisableAction();
        PlayerInputManager.Instance.DisableHudMachine();
        PlayerInputManager.Instance.DisableCodex();
        PlayerInputManager.Instance.DisableInventory();

        PlayerInputManager.Instance.EnableBuildMenu();
    }

    public void CloseBuilderMenu()
    {
        if (canvas.activeSelf)
        {
            crossAir.SetActive(true);
            Player.Instance.SetIsInMenu(false);

            canvas.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            PlayerInputManager.Instance.EnableMovement();
            PlayerInputManager.Instance.EnableHotbarAndBuild();
            PlayerInputManager.Instance.EnableMenuing();
            PlayerInputManager.Instance.EnableCamera();
            PlayerInputManager.Instance.EnableAction();
            PlayerInputManager.Instance.EnableDebugKeys();
            PlayerInputManager.Instance.EnableHudMachine();
            //PlayerInputManager.Instance.EnableCodex();
            PlayerInputManager.Instance.EnableInventory();

            PlayerInputManager.Instance.DisableBuildMenu();
        }
    }
    //public void WorkingTest()
    //{
    //    Debug.Log("ça marche");
    //}
}
