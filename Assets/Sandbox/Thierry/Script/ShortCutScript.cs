using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShortCutScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI build;
    [SerializeField] TextMeshProUGUI destroy;
    [SerializeField] TextMeshProUGUI scan;
    [SerializeField] TextMeshProUGUI inventory;
    [SerializeField] TextMeshProUGUI codex;
    PlayerInputManager playerInputManager;

    void Start()
    {
        playerInputManager =PlayerInputManager.Instance;
        UpdateText();
    }
    public void UpdateText()
    {
        build.text = playerInputManager.OpenBuildGetNameKey().ToUpper();
        destroy.text = playerInputManager.DestructionModeKeyName().ToUpper();
        scan.text = playerInputManager.ScannerKeyName().ToUpper();
        inventory.text = playerInputManager.OpenInventoryKeyName().ToUpper();
        codex.text = playerInputManager.OpenCodexKeyName().ToUpper();
    }
}
