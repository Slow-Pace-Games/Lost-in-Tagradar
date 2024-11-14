using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    #region Singleton
    private static PauseManager instance;
    public static PauseManager Instance { get => instance; }

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

    [Header("Pause Panel")]
    [SerializeField] private GameObject panelPause;

    [Header("Button")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        continueButton.onClick.AddListener(() => Player.Instance.Pause());
        saveButton.onClick.AddListener(() => SaveSystem.Instance.Save());
        settingButton.onClick.AddListener(() => SettingsManager.Instance.ShowMenuCanvas());
        menuButton.onClick.AddListener(() => Resources.Load<SODatabase>("Database").ResetData());
        menuButton.onClick.AddListener(() => LoadScene("Menu"));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    public void TogglePause(bool state)
    {
        panelPause.SetActive(state);
    }

    public void LoadScene(string scene) => SceneManager.LoadScene(scene);
}