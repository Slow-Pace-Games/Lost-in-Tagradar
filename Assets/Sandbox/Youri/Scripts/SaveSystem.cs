using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using File = System.IO.File;

public class SaveSystem : MonoBehaviour
{
    public delegate void SaveDelegate();
    public delegate void LoadDelegate();
    public delegate void OnLoadFinishDelegate();
    
    #region Singleton
    private static SaveSystem instance;
    public static SaveSystem Instance { get => instance; }

    private void Awake()
    {
        Initialize(SettingsManager.Instance.saveName);
        Resources.Load<SODatabase>("Database").InitData();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    #endregion

    public int loadCount = 0;
    public int saveCount = 0;

    public SaveDelegate OnSave;
    public LoadDelegate OnLoad;
    public OnLoadFinishDelegate OnLoadFinish;

    [Header("Save Parameter")]
    [SerializeField] ClassContainer classContainer;
    [SerializeField] InfoContainer infoContainer;
    [SerializeField] string sceneToLoad;

    float timeSpent;

    public ClassContainer ClassContainer { get { return classContainer; } }

    string saveDestination;
    string headerDestination;

    [Header("Debug")]
    [SerializeField] bool debug = true;

    private void Start()
    {
        StartTrackingTime();
        if (debug)
        {
            string jsonString = File.ReadAllText(saveDestination);

            if (jsonString == null)
            {
                return;
            }

            classContainer = JsonConvert.DeserializeObject<ClassContainer>(jsonString);

            if (classContainer == null)
            {
                classContainer = new ClassContainer();
            }



            OnLoad?.Invoke();
        }

        PlayerInputManager.Instance.SaveAction(Save, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.LoadAction(Load, PlayerInputManager.ActionType.Add);

        OnLoadFinish?.Invoke();
    }

    public void Initialize(string _saveName)
    {
        saveDestination = Application.persistentDataPath + "/Save" + _saveName + ".json";
        headerDestination = Application.persistentDataPath + "/Header" + _saveName + ".json";

        if (!File.Exists(saveDestination))
        {
            File.Create(saveDestination);
            //Debug.Log("Save[" + _saveIndex + "] Created");
        }

        classContainer = new ClassContainer();
    }

    public void Save()
    {
        OnSave?.Invoke();

        string JsonString = JsonConvert.SerializeObject(classContainer, Formatting.Indented);

        File.WriteAllText(saveDestination, JsonString);


        string jsonStringHeader = File.ReadAllText(headerDestination);
        infoContainer = JsonConvert.DeserializeObject<InfoContainer>(jsonStringHeader);
        infoContainer.saveInfo.dateTime = DateTime.Now;
        infoContainer.saveInfo.timeSpent = FormatTime(ParseTime(infoContainer.saveInfo.timeSpent) + GetElapsedTime());
        StartTrackingTime();
        jsonStringHeader = JsonConvert.SerializeObject(infoContainer, Formatting.Indented);
        File.WriteAllText(headerDestination, jsonStringHeader);

        ScreenShot screenShot = GetComponent<ScreenShot>();
        screenShot.TakeScreenshot(SettingsManager.Instance.saveName);
    }

    public void Load()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void StartTrackingTime()
    {
        timeSpent = Time.time;
    }
    float GetElapsedTime()
    {
        return Time.time - timeSpent;
    }
    public string FormatTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);

        return string.Format("{0:D2}h {1:D2}m {2:D2}s", hours, minutes, seconds);
    }

    // Convertir une chaîne de caractères en format "HHh MMm SSs" en nombre total de secondes
    public float ParseTime(string timeString)
    {
        MatchCollection matches = Regex.Matches(timeString, @"\d+");
        if (matches.Count != 3)
        {
            Debug.LogError("Le format de l'heure n'est pas valide. Utilisez le format 'HHh MMm SSs'.");
            return 0f;
        }

        int hours = int.Parse(matches[0].Value);
        int minutes = int.Parse(matches[1].Value);
        int seconds = int.Parse(matches[2].Value);

        return hours * 3600f + minutes * 60f + seconds;
    }
    private void OnDisable()
    {
        PlayerInputManager.Instance.SaveAction(Save, PlayerInputManager.ActionType.Remove);
        PlayerInputManager.Instance.LoadAction(Load, PlayerInputManager.ActionType.Remove);
    }
}