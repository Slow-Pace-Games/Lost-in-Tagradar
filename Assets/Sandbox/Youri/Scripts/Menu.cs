using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using File = System.IO.File;
using Newtonsoft.Json;
using System.IO;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;

    [SerializeField] GameObject continueButton;

    [SerializeField] GameObject prefabButton;
    [SerializeField] Transform saveButtonParent;
    [SerializeField] List<GameObject> ButtonList;
    public DateComparer dateComparer;

    List<string> saves;

    string headerDestination;
    [SerializeField] TMP_InputField nameInputField;

    InfoContainer infoContainer;

    public InfoContainer InfoContainer { get { return infoContainer; } }

    private void Start()
    {
        saves = GetComponent<SaveSearcher>().GetSavesNumber();
        ButtonList = new List<GameObject>();
        ShowSaveInformation();
        SettingsManager.Instance.OnToggleSettings += ToggleMainMenu;
        ShowContinueButton();
    }

    public void SetSettingsCanvas() => SettingsManager.Instance.ToggleSettings();
    private void ToggleMainMenu() => menuPanel.SetActive(!menuPanel.activeSelf);

    void LoadIfPossible(int _index)
    {
        if (ButtonList[_index])
        {
            Debug.Log("Save [" + _index + "] Loaded");

            LoadSaveFile(ButtonList[_index].GetComponent<SaveButton>().saveName);
        }
        else
        {
            Debug.Log("Save [" + _index + "] doesn't exist");
        }
    }

    public void ClickLoad(int _index)
    {
        LoadIfPossible(_index);
    }

    public void LoadSaveFile(string _saveName)
    {
        SettingsManager.Instance.saveName = _saveName;

        SceneManager.LoadScene("Game");
    }

    public void NewGame()
    {
        CreateHeader(ButtonList.Count);
        LoadSaveFile(nameInputField.text);
    }

    void CreateHeader(int _saveIndex)
    {
        headerDestination = Application.persistentDataPath + "/Header" + nameInputField.text + ".json";

        try
        {
            if (!File.Exists(headerDestination))
            {
                using (File.Create(headerDestination)) { }
                //Debug.Log("Header[" + _saveIndex + "] Created");
            }

            infoContainer = new InfoContainer();

            // Fournir les informations ici
            infoContainer.saveInfo.name = nameInputField.text;
            infoContainer.saveInfo.timeSpent = "00h:00m:00s";
            infoContainer.saveInfo.dateTime = DateTime.Now;


            string jsonString = JsonConvert.SerializeObject(infoContainer, Formatting.Indented);

            File.WriteAllText(headerDestination, jsonString);
            //Debug.Log(headerDestination);
        }
        catch (IOException ex)
        {
            Debug.LogError("An IO exception occurred: " + ex.Message);
        }
    }

    public void Continue()
    {
        if (ButtonList.Count > 0)
        {
            LoadSaveFile(ButtonList[0].GetComponent<SaveButton>().saveName);
        }
        else
        {
            Debug.Log(ButtonList.Count + " SaveFile");
        }
    }
    public void ShowContinueButton()
    {
        if (ButtonList.Count > 0)
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }
    public void ShowSaveInformation()
    {
        for (int i = 0; i < saves.Count; i++)
        {
            string save = saves[i].Replace("Save", "");
            CreateSaveButton(save, i);
        }
    }

    void GetHeaderInfo(string _saveName)
    {
        headerDestination = Application.persistentDataPath + "/Header" + _saveName + ".json";

        string jsonString = File.ReadAllText(headerDestination);
        if (jsonString == null)
        {
            return;
        }

        infoContainer = JsonConvert.DeserializeObject<InfoContainer>(jsonString);
        if (infoContainer == null)
        {
            infoContainer = new InfoContainer();
        }
    }

    void CreateSaveButton(string _saveName, int _index)
    {
        GameObject newButton = Instantiate(prefabButton, saveButtonParent);
        Button[] buttons = newButton.GetComponentsInChildren<Button>();
        newButton.GetComponent<SaveButton>().saveName = _saveName;
        ImageLoader loader = newButton.GetComponent<ImageLoader>();
        TextMeshProUGUI[] buttonTextComponent = newButton.GetComponentsInChildren<TextMeshProUGUI>();

        buttons[1].onClick.AddListener(() => DeleteSaveFile(_saveName));

        GetHeaderInfo(_saveName);
        string photoName = "preview_" + _saveName + ".png";
        string photoLocation = System.IO.Path.Combine(Application.persistentDataPath, photoName);
        if (File.Exists(photoLocation))
        {
            loader.LoadAndDisplayImage(photoLocation);
        }
        else
        {
            // Charger l'image par défaut depuis le dossier Resources
            string defaultPhotoName = "DefaultPreviewImage"; // Assurez-vous que c'est le nom correct de votre texture dans Resources
            Texture2D defaultTexture = Resources.Load<Texture2D>(defaultPhotoName);

            if (defaultTexture != null)
            {
                // Convertir la texture en bytes
                byte[] bytes = defaultTexture.EncodeToPNG();

                // Écrire les bytes dans le fichier
                File.WriteAllBytes(photoLocation, bytes);

                // Charger l'image par défaut
                loader.LoadAndDisplayImage(photoLocation);
            }
            else
            {
                Debug.LogError("Default texture not found in Resources folder.");
            }
        }


        buttonTextComponent[0].text = infoContainer.saveInfo.name;
        buttonTextComponent[1].text = infoContainer.saveInfo.timeSpent;
        buttonTextComponent[2].text = infoContainer.saveInfo.dateTime.ToString();

        buttons[0].onClick.AddListener(() => ClickLoad(_index));

        ButtonList.Add(newButton);

        ButtonList.Sort((button1, button2) =>
        {
            DateTime date1 = GetDateTimeFromButton(button1);
            DateTime date2 = GetDateTimeFromButton(button2);
            return date1.CompareTo(date2);
        });

        foreach (GameObject button in ButtonList)
        {
            button.transform.SetAsLastSibling();
        }
    }

    DateTime GetDateTimeFromButton(GameObject button)
    {
        TextMeshProUGUI buttonTextComponent = button.GetComponentInChildren<TextMeshProUGUI>();
        string[] lines = buttonTextComponent.text.Split('\n');
        string lastLine = lines[lines.Length - 1];
        string dateTimeString = lastLine.Substring(lastLine.IndexOf(':') + 2);
        return DateTime.Parse(dateTimeString);
    }

    public void DeleteSaveFile(string _name)
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            string[] fichiers = Directory.GetFiles(Application.persistentDataPath);

            foreach (string fichier in fichiers)
            {
                if (Path.GetFileName(fichier).Contains(_name))
                {
                    File.Delete(fichier);
                    Debug.Log("Fichier supprimé : " + fichier);
                }
            }

            // Actualise les saves disponible
            int childCount = saveButtonParent.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(saveButtonParent.transform.GetChild(i).gameObject);
            }

            ButtonList = new List<GameObject>();
            saves = GetComponent<SaveSearcher>().GetSavesNumber();
            ShowSaveInformation();
            ShowContinueButton();
        }
        else
        {
            Debug.LogError("Le dossier n'existe pas : " + Application.persistentDataPath);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.settingsToSaveDelegate -= ToggleMainMenu;
    }
}