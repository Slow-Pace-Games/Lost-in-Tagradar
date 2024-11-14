using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ToSaveDataBase", menuName = "Sandbox/Youri/ToSaveDataBase")]
public class ToSaveDataBase : ScriptableObject/*, ISerializationCallbackReceiver*/
{
    public List<GameObject> ToSaveObjects;
    public Dictionary<GameObject, int> GetID = new Dictionary<GameObject, int>();


    public void OnAfterDeserialize()
    {
        GetID = new Dictionary<GameObject, int>();

        if (ToSaveObjects != null)
        {
            for (int i = 0; i < ToSaveObjects.Count; i++)
            {
                GetID.Add(ToSaveObjects[i], i);
            }
        }
    }


    public void OnBeforeSerialize()
    {
    }
}