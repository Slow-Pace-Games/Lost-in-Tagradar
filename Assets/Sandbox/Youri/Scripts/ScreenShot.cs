using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    public GameObject panelToIgnore; // Référence au GameObject à ignorer
    private string screenshotName; // Index du screenshot en cours

    // Méthode pour prendre un screenshot
    public void TakeScreenshot(string _name)
    {
        screenshotName = _name;

        panelToIgnore.SetActive(false);

        Invoke("CaptureScreenshot", 0.1f);
    }

    private void CaptureScreenshot()
    {
        string screenshotFolder = Application.persistentDataPath;

        string fileName = "preview_" + screenshotName + ".png";
        string filePath = System.IO.Path.Combine(screenshotFolder, fileName);
        ScreenCapture.CaptureScreenshot(filePath);

        Invoke("ReactivateGameObject", 0.1f);
    }

    private void ReactivateGameObject()
    {
        panelToIgnore.SetActive(true);
    }
}
