using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "DataEnvironment", menuName = "GPU Environment", order = 0)]
public class SODataEnvironment : ScriptableObject
{
    public List<GPUDictionary> dataEnvironement;

    public void AddNewItems(GPURenderer gpuRenderer, List<Matrix4x4> matrixTransform)
    {
        int index = Contains(gpuRenderer);

        if (index == -1)
            dataEnvironement.Add(new GPUDictionary(gpuRenderer, matrixTransform));
        else
        {
            dataEnvironement[index].AddRangeMatrixTransform(matrixTransform);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.Refresh();
#endif
    }

    public void AddNewItem(GPURenderer gpuRenderer, Matrix4x4 matrixTransform)
    {
        int index = Contains(gpuRenderer);

        if (index == -1)
            dataEnvironement.Add(new GPUDictionary(gpuRenderer, new List<Matrix4x4>() { matrixTransform }));
        else
        {
            dataEnvironement[index].AddMatrixTransform(matrixTransform);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.Refresh();
#endif
    }

    private int Contains(GPURenderer gpuRenderer)
    {
        for (int i = 0; i < dataEnvironement.Count; i++)
        {
            if (dataEnvironement[i].renderer.Equals(gpuRenderer))
                return i;
        }

        return -1;
    }
}

[System.Serializable]
public class GPUDictionary
{
    public GPURenderer renderer;
    public List<Matrix4x4> matrixTransform = new List<Matrix4x4>();

    public void AddRangeMatrixTransform(List<Matrix4x4> matrixTransform)
    {
        this.matrixTransform.AddRange(matrixTransform);
    }

    public void AddMatrixTransform(Matrix4x4 matrixTransform)
    {
        this.matrixTransform.Add(matrixTransform);
    }

    public GPUDictionary(GPURenderer renderer, List<Matrix4x4> matrixTransform)
    {
        this.renderer = renderer;
        this.matrixTransform = matrixTransform;
    }
}

[System.Serializable]
public class GPURenderer
{
    public Mesh mesh;
    public Material material;

    public GPURenderer(Mesh mesh, Material material)
    {
        this.mesh = mesh;
        this.material = material;
    }

    public bool Equals(GPURenderer other)
    {
        return mesh == other.mesh && material == other.material;
    }
}