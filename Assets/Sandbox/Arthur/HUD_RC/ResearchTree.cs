using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTree : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SODatabase database;
    [SerializeField] private List<SOResearch> research;

    [Header("Buttons")]
    [SerializeField] private List<Button> researchButtons;
    private List<Button> researchLeft;
    [SerializeField] private Button unlockButton;
    private int idSelectedResearch;

    [Header("Elements to show")]
    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text reward;
    [SerializeField] private Image rewardSprite;
    [SerializeField] private List<IconItemWithQuantity> itemsToShow;

    public bool isResearching = false;
    private float researchTimer = 0;

    private void Start()
    {
        researchLeft = researchButtons;
    }

    public void UpdateResearchDuration()
    {
        if (researchTimer > 0)
        {
            researchTimer -= Time.deltaTime;
        }
        
        if (researchTimer <= 0)
        {
            isResearching = false;
            research[idSelectedResearch].isUnlocked = true;
            database.AllRCRecipes[idSelectedResearch].isUnlock = true;
        }

        time.text = UpdateHUDTimer();
    }

    private string UpdateHUDTimer()
    {
        string duration;
        string strMinutes = "";
        string strSeconds = "";
        int minutes = 0;
        int seconds = 0;

        minutes = (int)researchTimer / 60;
        seconds = (int)researchTimer % 60;
        if (minutes < 10)
        {
            strMinutes = "0";
        }
        if (seconds < 10)
        {
            strSeconds = "0";
        }
        strMinutes += minutes.ToString();
        strSeconds += seconds.ToString();
        duration = strMinutes + " : " + strSeconds;

        return duration;
    }

    private string GetResearchDuration(int id)
    {
        string duration;
        string strMinutes = "";
        string strSeconds = "";
        int minutes = 0;
        int seconds = 0;

        minutes = research[id].Duration / 60;
        seconds = research[id].Duration % 60;
        if (minutes < 10)
        {
            strMinutes = "0";
        }
        if (seconds < 10)
        { 
            strSeconds = "0";
        }
        strMinutes += minutes.ToString();
        strSeconds += seconds.ToString();
        duration = strMinutes + " : " + strSeconds;

        return duration;
    }

    public void ResearchClick(int idButton)
    {
        foreach (IconItemWithQuantity item in itemsToShow)
        {
            item.gameObject.SetActive(true);
        }
        time.gameObject.SetActive(true);
        time.text = GetResearchDuration(idButton);
        reward.gameObject.SetActive(true);
        reward.text = research[idButton].Reward;
        rewardSprite.gameObject.SetActive(true);
        rewardSprite.sprite = database.AllRCRecipes[idButton].ItemOutput.Sprite;
        unlockButton.gameObject.SetActive(true);
        if (!isResearching)
        {
            unlockButton.interactable = true;
        }
        idSelectedResearch = idButton;
        itemsToShow[0].Init(research[idButton].Resources[0].Sprite, research[idButton].Quantities[0], Player.Instance.GetItemAmount(research[idButton].Resources[0]));
        itemsToShow[1].Init(research[idButton].Resources[1].Sprite, research[idButton].Quantities[1], Player.Instance.GetItemAmount(research[idButton].Resources[1]));
    }

    private void UnlockClick()
    {
        for (int i = 0; i < research[idSelectedResearch].Resources.Count; ++i)
        {
            SOItems item = research[idSelectedResearch].Resources[i];
            if (Player.Instance.GetItemAmount(item) < research[idSelectedResearch].Quantities[i])
            {
                Debug.Log("Not Enough Minerals");
                return;
            }
        }

        unlockButton.interactable = false;
        isResearching = true;
        researchTimer = research[idSelectedResearch].Duration;
        researchButtons[idSelectedResearch].interactable = false;

        for (int i = 0; i < research[idSelectedResearch].Resources.Count; ++i)
        {
            SOItems item = research[idSelectedResearch].Resources[i];
            Player.Instance.RemoveItem(item, research[idSelectedResearch].Quantities[i]);
        }
    }

    public void InitResearchTree()
    {
        unlockButton.onClick.AddListener(() => UnlockClick());
    }
}
