using UnityEngine;

public class DatabaseContainer : SaveContainer
{
    [SerializeField] private SODatabase database;

    protected override void Convert()
    {
        SaveSystem.Instance.ClassContainer.database = database.SaveData();
    }

    protected override void Load()
    {
        database.LoadData(SaveSystem.Instance.ClassContainer.database);
    }
}