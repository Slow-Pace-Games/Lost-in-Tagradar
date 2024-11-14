using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoreMenu : MonoBehaviour
{
    [SerializeField] GameObject leftPanel;
    [SerializeField] TextMeshProUGUI loreTitle;
    [SerializeField] TextMeshProUGUI loreDescription;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newInBox = Instantiate(Codex.Instance.ObjectTitlePrefab, leftPanel.transform);
        }
    }
}
