using System.Collections.Generic;
using UnityEngine;

//possiblement un autre container pour enregistrer les bool dans le SOItem pour les première découverte et les SOMilestone pour le unlock ou pas

public class SaveContainer : MonoBehaviour
{
    [SerializeField] protected Dictionary<string, int> dictionaryPrefabs = new Dictionary<string, int>();
    [SerializeField] protected List<GameObject> prefabs;
    private void Awake()
    {
        SaveSystem.Instance.OnLoad += Load;
        SaveSystem.Instance.OnSave += Convert;

        for (int i = 0; i < prefabs.Count; i++)
        {
            dictionaryPrefabs.Add(prefabs[i].name + "(Clone)", i);
        }
    }

    protected virtual void Convert()
    {

    }

    protected virtual void Load()
    {

    }
}