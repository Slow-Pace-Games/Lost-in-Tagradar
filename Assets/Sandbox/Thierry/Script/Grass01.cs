using UnityEngine;

public class GrassType01 : MonoBehaviour
{
    void OnEnable() => GrassManager.grass01.Add(gameObject.transform);
    void OnDisable() => GrassManager.grass01.Remove(gameObject.transform);
}