using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using File = System.IO.File;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;

    public void ToggleSettings()
    {
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
        OnToggleSettings?.Invoke();
    }

    public TMP_Dropdown resolutionDropdown;

    public string saveName = "null name";


    int frameRateLimit = 60;
    bool isVsyncOn = true;

    [Header("UI")]
    [SerializeField] Slider sensitivitySliderX;
    [SerializeField] TMP_Text sensitivityTextX;
    [SerializeField] Slider sensitivitySliderY;
    [SerializeField] TMP_Text sensitivityTextY;

    [SerializeField] Toggle DisplayToggle;
    [SerializeField] Toggle VsyncToggle;

    public SettingsToSave settingsToSave;
    string destination;

    [SerializeField] TextAsset DefaultSettings;

    [Header("Sound")]
    [SerializeField] Slider generalVolume;
    [SerializeField] Slider music;
    [SerializeField] Slider soundEffect;

    [SerializeField] AudioMixer mixer;

    public delegate void SettingsToSaveDelegate();
    public delegate void ToggleSettingsEvent();

    #region Singleton
    private static SettingsManager instance;
    public static SettingsManager Instance { get => instance; }

    private void Awake()
    {
        Initialize();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public SettingsToSaveDelegate settingsToSaveDelegate;
    public ToggleSettingsEvent OnToggleSettings;

    private void Start()
    {
        GetScreenResolutions();
        Load();

        Apply();
    }

    private void Update()
    {
        ShowSensitivity();
    }

    public void ShowMenuCanvas()
    {
        ToggleSettings();
    }

    void GetScreenResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        TMP_Dropdown.OptionDataList options = new TMP_Dropdown.OptionDataList();

        foreach (Resolution resolution in resolutions)
        {
            options.options.Add(new TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height + " : " + Mathf.FloorToInt((float)resolution.refreshRateRatio.value) + " Hz"));
        }

        resolutionDropdown.options = options.options;

        if (resolutions.Length > 0)
        {
            resolutionDropdown.value = resolutions.Length - 1;
        }
        else
        {
            Debug.LogWarning("Aucune résolution disponible.");
        }
    }

    public void ApplyResolution()
    {
        int selectedResolutionIndex = resolutionDropdown.value;

        if (settingsToSave != null)
            settingsToSave.graphics.screenResIndex = resolutionDropdown.value;

        Resolution[] resolutions = Screen.resolutions;
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

        frameRateLimit = Mathf.FloorToInt((float)selectedResolution.refreshRateRatio.value);
    }

    public void SetDisplay()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void SetVSync()
    {
        isVsyncOn = !isVsyncOn;
    }

    void ApplyVsync()
    {
        if (isVsyncOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        VsyncToggle.isOn = isVsyncOn;
        if (settingsToSave != null)
            settingsToSave.graphics.isVsyncOn = isVsyncOn;
    }

    public void Save()
    {
        string JsonString = JsonConvert.SerializeObject(settingsToSave, Formatting.Indented);

        File.WriteAllText(destination, JsonString);
    }

    void Load()
    {
        string jsonString = File.ReadAllText(destination);

        if (jsonString == null)
        {
            return;
        }


        settingsToSave = JsonConvert.DeserializeObject<SettingsToSave>(jsonString);
        if (settingsToSave != null)
        {

            sensitivitySliderX.value = settingsToSave.playerSetSaveable.mouseSensitivityX;
            sensitivitySliderY.value = settingsToSave.playerSetSaveable.mouseSensitivityY;

            resolutionDropdown.value = settingsToSave.graphics.screenResIndex;

            isVsyncOn = settingsToSave.graphics.isVsyncOn;

            DisplayToggle.isOn = Screen.fullScreen;

            settingsToSave = new SettingsToSave();
        }
    }

    public void Apply()
    {
        if (settingsToSave != null)
        {
            settingsToSave.playerSetSaveable.mouseSensitivityX = sensitivitySliderX.value;
            settingsToSave.playerSetSaveable.mouseSensitivityY = sensitivitySliderY.value;
            if (Player.Instance != null)
            {
                Player.Instance.SetSensX(settingsToSave.playerSetSaveable.mouseSensitivityX);
                Player.Instance.SetSensY(settingsToSave.playerSetSaveable.mouseSensitivityY);
            }
        }

        ApplyResolution();
        ApplyVsync();
        Save();
        AudioManager();
    }

    public void Back()
    {
        ToggleSettings();
        settingsToSaveDelegate?.Invoke();
    }

    void ShowSensitivity()
    {
        sensitivityTextX.text = "X Sensitivity : " + sensitivitySliderX.value.ToString();
        sensitivityTextY.text = "Y Sensitivity : " + sensitivitySliderY.value.ToString();
    }

    void Initialize()
    {
        destination = Application.persistentDataPath + "/Settings.json";

        if (!File.Exists(destination))
        {
            StreamWriter tempStream = File.CreateText(destination);
            tempStream.Write(DefaultSettings.text);
            tempStream.Close();
        }
    }

    void AudioManager()
    {
        //PARFAIT
        if (generalVolume.value != 0 && soundEffect.value != 0)
        {
            mixer.SetFloat("Ambient", -12f * 1 / generalVolume.value * 1 / soundEffect.value);
            mixer.SetFloat("Interface", -6f * 1 / generalVolume.value * 1 / soundEffect.value);
            mixer.SetFloat("Main", -4f * 1 / generalVolume.value * 1 / soundEffect.value);
            mixer.SetFloat("Secondary", -6f * 1 / generalVolume.value * 1 / soundEffect.value);
        }
        else
        {
            mixer.SetFloat("Ambient", -80);
            mixer.SetFloat("Interface", -80);
            mixer.SetFloat("Main", -80);
            mixer.SetFloat("Secondary", -80);
        }

        if (generalVolume.value != 0 && music.value != 0)
            mixer.SetFloat("Musics", -12f * 1 / generalVolume.value * 1 / music.value);
        else
            mixer.SetFloat("Musics", -80);



    }
}