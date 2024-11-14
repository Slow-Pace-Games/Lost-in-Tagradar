using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInfo : MonoBehaviour
{
    SaveInfoSaveable saveInfoSaveable = new SaveInfoSaveable();

    public void Convert()
    {
    }

    public void Load()
    {
    }
}

[System.Serializable]
public class SaveInfoSaveable
{
    public string name;
    public string timeSpent;
    public DateTime dateTime;
}
