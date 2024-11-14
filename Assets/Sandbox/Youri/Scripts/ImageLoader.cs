using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public RawImage imageDisplay;

    // Méthode pour charger une image depuis le système de fichiers et l'afficher
    public void LoadAndDisplayImage(string imagePath)
    {
        // Vérifie si le fichier existe
        if (System.IO.File.Exists(imagePath))
        {
            // Charge l'image depuis le chemin spécifié
            byte[] fileData = System.IO.File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // Affiche l'image dans l'objet RawImage
            imageDisplay.texture = texture;
        }
        else
        {
            Debug.LogError("Image not found at path: " + imagePath);
        }
    }
}
