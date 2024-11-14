using UnityEngine;
using UnityEngine.UI;

public class CheatEngine : MonoBehaviour
{
    #region Singleton
    private static CheatEngine instance;
    public static CheatEngine Instance { get => instance; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
    #endregion
    public enum CheatState
    {
        Game,
        Player,
        Inventory,
        Milestone,
        SaveLoad,
    }
    public enum TabState
    {
        Normal,
        Enter,
        Select,
    }

    private CheatState state = CheatState.Game;
    private bool isOpen = false;
    public delegate void OnClickEvent(CheatState state);
    public OnClickEvent onClickEvent;
    private GameObject canvas;

    [Header("Tab bar")]
    [SerializeField] private TabCheatButton gameplayTab;
    [SerializeField] private TabCheatButton playerTab;
    [SerializeField] private TabCheatButton inventoryTab;
    [SerializeField] private TabCheatButton milestoneTab;
    [SerializeField] private TabCheatButton saveLoadTab;
    [SerializeField] private Button cross;

    [Header("Tab Color")]
    [SerializeField] private Color normal;
    [SerializeField] private Color enter;
    [SerializeField] private Color select;

    public Color TabColor(TabState tabState)
    {
        switch (tabState)
        {
            case TabState.Normal:
                return normal;

            case TabState.Enter:
                return enter;

            case TabState.Select:
                return select;

            default:
                Debug.LogError("No color for tab button in cheat Engine");
                return normal;
        }
    }

    private void Start()
    {
        onClickEvent += SwitchWindow;
        canvas = transform.GetChild(0).gameObject;
        canvas.SetActive(false);
        cross.onClick.AddListener(ToggleCheatEngine);
        PlayerInputManager.Instance.CheatEngineAction(ToggleCheatEngine, PlayerInputManager.ActionType.Add);

        gameplayTab.Init();
        playerTab.Init();
        inventoryTab.Init();
        milestoneTab.Init();
        saveLoadTab.Init();
    }

    private void SwitchWindow(CheatState tabState)
    {
        CloseAllWindow();
        switch (tabState)
        {
            case CheatState.Game:
                OpenGameWindow();
                break;

            case CheatState.Player:
                OpenPlayerWindow();
                break;

            case CheatState.Inventory:
                OpenInventoryWindow();
                break;

            case CheatState.Milestone:
                OpenMilestoneWindow();
                break;

            case CheatState.SaveLoad:
                OpenSaveLoadWindow();
                break;

            default:
                Debug.LogError("Cheat Engine Switch Error");
                break;
        }

    }
    private void ToggleCheatEngine()
    {
        if (!isOpen)
        {
            isOpen = true;
            canvas.SetActive(true);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            PlayerInputManager.Instance.DisableMovement();
            PlayerInputManager.Instance.DisableBuild();
            PlayerInputManager.Instance.DisableMenuing();
            PlayerInputManager.Instance.DisableCamera();
            PlayerInputManager.Instance.DisableAction();
            PlayerInputManager.Instance.DisableInventory();

            PlayerInputManager.Instance.EnableDebugKeys();

            SwitchWindow(state);
        }
        else
        {
            isOpen = false;
            canvas.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            PlayerInputManager.Instance.EnableMovement();
            PlayerInputManager.Instance.EnableBuild();
            PlayerInputManager.Instance.EnableMenuing();
            PlayerInputManager.Instance.EnableCamera();
            PlayerInputManager.Instance.EnableAction();
            PlayerInputManager.Instance.EnableDebugKeys();

            PlayerInputManager.Instance.DisableInventory();
        }
    }
    private void CloseAllWindow()
    {
        gameplayTab.CloseTabWindow();
        playerTab.CloseTabWindow();
        inventoryTab.CloseTabWindow();
        milestoneTab.CloseTabWindow();
        saveLoadTab.CloseTabWindow();
    }
    private void OpenGameWindow()
    {
        state = CheatState.Game;
        gameplayTab.OpenTabWindow();
    }
    private void OpenPlayerWindow()
    {
        state = CheatState.Player;
        playerTab.OpenTabWindow();
    }
    private void OpenInventoryWindow()
    {
        state = CheatState.Inventory;
        inventoryTab.OpenTabWindow();
    }
    private void OpenMilestoneWindow()
    {
        state = CheatState.Milestone;
        milestoneTab.OpenTabWindow();
    }
    private void OpenSaveLoadWindow()
    {
        state = CheatState.SaveLoad;
        saveLoadTab.OpenTabWindow();
    }
}