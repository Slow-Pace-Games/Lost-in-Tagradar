using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanUi : MonoBehaviour
{
    public void OpenRessourceSelectionMenu()
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<HUDScannerRessource>().OnHoverImage(new Color(1, 1, 1, 1f));
        }
        PlayerInputManager.Instance.DisableCamera();
    }

    public void CloseRessourceSelectionMenu()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInputManager.Instance.EnableCamera();
    }
}
