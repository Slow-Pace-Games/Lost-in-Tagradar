using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class DisplayOrderTriangle : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] bool fisrt = false;

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = mesh.uv;

        if (fisrt)
        {
            for (int i = 2; i < vertices.Length; i += 4)
            {
                Handles.color = Color.red;
                Handles.Label(transform.position + vertices[i] + normals[i] * 0.3f, "  " + i.ToString());
                Handles.Label(transform.position + vertices[i + 1] + normals[i + 1] * 0.3f, "  " + (i + 1).ToString());
            }
        }
    }
}
#endif