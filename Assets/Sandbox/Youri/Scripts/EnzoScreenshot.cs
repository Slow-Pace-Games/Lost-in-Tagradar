using UnityEngine;

public class EnzoScreenshot : MonoBehaviour
{
    int index = 0;
    [SerializeField] int ratio = 1;
    void Update()
    {
        // V�rifier si la touche "P" est enfonc�e
        if (Input.GetKeyDown(KeyCode.P))
        {
            // G�n�rer le chemin d'enregistrement avec le nom du fichier
            string path = Application.persistentDataPath + "/ScreenShot" + index + ".png";

            // Capturer le screenshot et l'enregistrer
            ScreenCapture.CaptureScreenshot(path, ratio);
            index++;
        }
    }
}
