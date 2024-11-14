using System.Collections.Generic;
using UnityEngine;

//deux choix
//  -on enlève les blocs sur les ressources
//  -on enregistre un bool de cassé ou pas

public class ResourceContainer : SaveContainer
{
    //List convertie des enfants du gameobject (container)
    [SerializeField] private List<ResourceSaveable> resources = new List<ResourceSaveable>();

    [SerializeField] List<Transform> allResource;

    protected override void Convert()
    {
        resources.Clear();

        for (int i = 0; i < allResource.Count; i++)
        {
            for (int j = 0; j < allResource[i].transform.childCount; j++) // Iron folder
            {
                for (int k = 0; k < allResource[i].transform.GetChild(j).childCount; k++) // les prefabs
                {
                    for (int l = 0; l < allResource[i].transform.GetChild(j).GetChild(k).childCount; l++) // gisement et minerai
                    {
                        ResourceSaveable resourceSaveable = new ResourceSaveable();
                        resourceSaveable.level = i;
                        resourceSaveable.name = allResource[i].transform.GetChild(j).GetChild(k).GetChild(l).name;
                        if (l == 1)
                        {
                            if (!allResource[i].transform.GetChild(j).GetChild(k).GetChild(l).gameObject.activeSelf)
                            {
                                resourceSaveable.hasBeenHarvested = true;
                            }
                            else
                            {
                                resourceSaveable.hasBeenHarvested = false;
                            }
                            resources.Add(resourceSaveable);
                        }
                    }
                }
            }
        }


        SaveSystem.Instance.ClassContainer.resourcesList = resources;
    }

    protected override void Load()
    {
        resources = SaveSystem.Instance.ClassContainer.resourcesList;
        int indexResource = 0;
        if (resources.Count > 0)
        {
            for (int i = 0; i < allResource.Count; i++)
            {
                for (int j = 0; j < allResource[i].childCount; j++)
                {
                    for (int k = 0; k < allResource[i].GetChild(j).childCount; k++)
                    {
                        for (int l = 0; l < allResource[i].GetChild(j).GetChild(k).childCount; l++)
                        {
                            if (l == 1)
                            {
                                if (indexResource < resources.Count && resources[indexResource].hasBeenHarvested)
                                {
                                    allResource[i].GetChild(j).GetChild(k).GetChild(l).gameObject.SetActive(false);
                                }
                                indexResource++;
                            }
                        }
                    }
                }
            }
        }
    }


}



[System.Serializable]
public class ResourceSaveable
{
    public int key;
    public string name;
    public int level;

    public bool hasBeenHarvested;

    public TransformSerialized transform;
}

