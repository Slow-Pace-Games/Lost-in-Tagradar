using System.Collections.Generic;
using UnityEngine;

public class SOBrushProfiles : ScriptableObject
{
    public List<CPUProfile> cpuProfiles = new List<CPUProfile>();
    public List<GPUProfile> gpuProfiles = new List<GPUProfile>();
}

[System.Serializable]
public class CPUProfile
{
    public float radius;
    public float spawnCount;

    public GameObject prefabInstance;

    public CPUProfile(float radius, float spawnCount, GameObject prefabInstance)
    {
        this.radius = radius;
        this.spawnCount = spawnCount;
        this.prefabInstance = prefabInstance;
    }
}

[System.Serializable]
public class GPUProfile
{

}