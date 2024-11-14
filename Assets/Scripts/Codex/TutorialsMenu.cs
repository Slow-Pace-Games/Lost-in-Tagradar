using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialsMenu : MonoBehaviour
{
    [SerializeField] GameObject leftPanel;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] SOLoidDatabase loidDatabase;

    private void Start()
    {
        InitMenu();
    }

    public void InitMenu()
    {
        ResetMenu();

        // Create left panel titles and configure their buttons
        for (int i = 0; i < loidDatabase.Tuto.Length; i++)
        {
            // Add tuto titles except tuto finish
            if (loidDatabase.Tuto[i].TutoTitle != "Tuto : Finish")
            {
                // Instantiate new title
                GameObject newTuto = Instantiate(Codex.Instance.ObjectTitlePrefab, leftPanel.transform);
                // Destroy title image
                Destroy(newTuto.transform.GetChild(0).GetComponent<Image>());
                // Init title text
                newTuto.GetComponentInChildren<TextMeshProUGUI>().text = loidDatabase.Tuto[i].TutoTitle;
                // translate text on the left because of the image suppression
                Vector3 newPos = new Vector3(25f, 0f, 0f);
                newTuto.GetComponentInChildren<TextMeshProUGUI>().transform.position -= newPos;
                // Add function for changing description when button is clicking
                int index = i;
                newTuto.GetComponentInChildren<Button>().onClick.AddListener(() => ChangeDescription(loidDatabase.Tuto[index].TutoDescription));
            }
        }
    }

    private void ResetMenu()
    {
        for (int i = 0; i < leftPanel.transform.childCount; i++)
        {
            Destroy(leftPanel.transform.GetChild(i).gameObject);
        }

        description.text = string.Empty;
    }

    private void ChangeDescription(string descritption)
    {
        description.text = descritption;
    }
}
