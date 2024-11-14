using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWarning : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Image background;

    private void Start()
    {
        message.enabled = false;
        background.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            InvertEnable();
        }
    }

    private void InvertEnable()
    {
        message.enabled = !message.enabled;
        background.enabled = !background.enabled;
    }
}
