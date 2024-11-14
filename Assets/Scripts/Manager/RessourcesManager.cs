using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourcesManager : MonoBehaviour
{
    public static RessourcesManager instance;

    public List<RessourceData> listData;
    public GameObject[] arrayPrefabs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        listData = new List<RessourceData>();
    }

    private void Update()
    {
        if (listData.Count > 0)
        {
            for (int i = 0; i < listData.Count; i++)
            {
                listData[i].timer -= Time.deltaTime;

                if (listData[i].timer < 0)
                {
                    Instantiate(arrayPrefabs[(int)listData[i].type], listData[i].position, listData[i].rotation);
                    listData.RemoveAt(i);
                }
            }
        }
    }
}
