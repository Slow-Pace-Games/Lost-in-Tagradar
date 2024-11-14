using UnityEngine;
using UnityEngine.EventSystems;

public class Codex : MonoBehaviour
{
    private static Codex instance;

    [Header("Data")]
    [SerializeField] GameObject canvas;
    [SerializeField] SODatabase database;

    [Header("Object Prefabs")]
    [SerializeField] GameObject objectTitlePrefab;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject crossAir;

    [Header("Submenu Prefabs")]
    [SerializeField] GameObject inBoxMenu;
    [SerializeField] GameObject tutoMenu;
    [SerializeField] GameObject codexMenu;

    private GameObject activePanel;

    #region Getters and Setters
    public static Codex Instance { get { return instance; } }
    public GameObject Canvas { get { return canvas; } }
    public GameObject ObjectTitlePrefab { get { return objectTitlePrefab; } }
    public GameObject ArrowPrefab { get { return arrowPrefab; } }
    public SODatabase Database { get { return database; } }
    public CodexMenu CodexMenu { get { return codexMenu.GetComponent<CodexMenu>(); } }
    #endregion


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

    // Start is called before the first frame update
    private void Start()
    {
        InitPanels();

        PlayerInputManager.Instance.OpenCodex(OpenCodex, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.CloseCodex(CloseCodex, PlayerInputManager.ActionType.Add);
    }

    private void InitPanels()
    {
        activePanel = codexMenu;
        inBoxMenu.SetActive(false);
        tutoMenu.SetActive(false);
        codexMenu.SetActive(true);
    }

    public void ActiveMenu(GameObject menu)
    {
        activePanel.SetActive(false);
        menu.SetActive(true);
        activePanel = menu;
    }

    public void OpenOrCloseCodex()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (!canvas.activeSelf)
        {
            OpenCodex();
        }
        else
        {
            CloseCodex();
        }
    }

    public void DestroyContent(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public void OpenCodex()
    {
        ActiveMenu(codexMenu);
        Loid.Instance.UpdateTuto(PlayerAction.OpenCodex);

        crossAir.SetActive(false);
        Player.Instance.SetIsInMenu(true);

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
        PlayerInputManager.Instance.DisableBuildMenu();
        PlayerInputManager.Instance.DisableInventory();

        PlayerInputManager.Instance.EnableCodex();
    }

    private void CloseCodex()
    {
        if (canvas == null) canvas = transform.GetChild(0).gameObject;

        if (canvas.activeSelf)
        {
            crossAir.SetActive(true);
            Player.Instance.SetIsInMenu(false);

            CodexMenu menu = codexMenu.GetComponent<CodexMenu>();
            menu.ActiveRecipeContainer(false);
            menu.ActiveRecipeDescriptionContainer(false);
            DestroyContent(menu.middlePanel.transform.GetChild(0).transform);
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
            PlayerInputManager.Instance.EnableBuildMenu();
            PlayerInputManager.Instance.EnableInventory();

            PlayerInputManager.Instance.DisableCodex();
        }
    }
}
