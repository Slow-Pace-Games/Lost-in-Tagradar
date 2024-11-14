using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSearcher : MonoBehaviour
{
    public List<string> GetSavesNumber()
    {
        string directoryPath = Application.persistentDataPath;
        List<string> jsonFiles = new List<string>();

        string[] files = Directory.GetFiles(directoryPath);
        foreach (string file in files)
        {
            if (Path.GetExtension(file).Equals(".json", System.StringComparison.OrdinalIgnoreCase))
            {
                if (Path.GetFileNameWithoutExtension(file).Contains("Save"))
                {
                    jsonFiles.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        return jsonFiles;
    }
}
