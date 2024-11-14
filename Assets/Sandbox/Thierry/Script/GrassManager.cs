using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrassManager : MonoBehaviour
{
    Mesh mesh;
    [SerializeField] Material material;

    public static List<Transform> grass01 = new List<Transform>();

    SODataEnvironment environment;
    List<Matrix4x4> matrixList = new List<Matrix4x4>();


    private void Start()
    {
        environment = Resources.Load("DataEnvironmentGame") as SODataEnvironment;
        while (grass01.Count > 0)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(grass01[0].position, grass01[0].rotation, grass01[0].parent.parent.localScale);
            mesh = grass01[0].GetComponent<MeshFilter>().sharedMesh;
            Debug.Log(grass01[0].parent.parent.localScale);
            matrixList = new List<Matrix4x4>();
            matrixList.Add(matrix);

            environment.AddNewItems(new GPURenderer(mesh, material), matrixList);
            Destroy(grass01[0].gameObject);
        }
    }
}
