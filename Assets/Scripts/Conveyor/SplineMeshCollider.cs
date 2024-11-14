using Cinemachine.Utility;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshCollider : MonoBehaviour
{
    private List<BoxCollider> splineCollider = new List<BoxCollider>();
    private Mesh mesh;

    public void UpdateColliders(Mesh baizeMesh, bool endPoint = false)
    {
        mesh = baizeMesh;

        int listIndex = 0;
        for (int indexMesh = 0; indexMesh < mesh.vertexCount; indexMesh += 4)
        {
            if (endPoint && listIndex < 5)
            {
                TriggerCollision(listIndex);
            }
            else
            {
                if (listIndex >= splineCollider.Count)//no collider exist
                    CreateCollider(indexMesh);
                else                                  //collider exist
                    UpdateCollider(indexMesh, listIndex);
            }

            listIndex++;
        }

        //too much collider
        int maxIndexList = splineCollider.Count - 1;
        if (listIndex < maxIndexList)
        {
            int numberToDelete = maxIndexList - listIndex;
            for (int i = 0; i < numberToDelete; i++)
            {
                DestroyCollider(splineCollider.Count - 1);
            }
        }
    }
    public void SetSplineBuild()
    {
        DestroyAllColliders();

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        meshCollider.excludeLayers = LayerMask.GetMask("Items", "IO", "TransparentFX", "Ignore Raycast", "Ground", "Water", "UI", "Selectable", "Deposit", "Monster", "ConveySpawn", "CAILLOUX", "Electricity");

        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void CreateCollider(int indexMesh)
    {
        if (mesh.vertexCount <= indexMesh + 6)
            return;

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        Bounds colliderBounds = new Bounds();

        colliderBounds.min = mesh.vertices[indexMesh];
        colliderBounds.max = mesh.vertices[indexMesh + 6];

        boxCollider.center = colliderBounds.center;
        boxCollider.size = colliderBounds.size.Abs();

        boxCollider.isTrigger = false;
        boxCollider.excludeLayers = LayerMask.GetMask("ConveySpawn", "IO", "Player");

        splineCollider.Add(boxCollider);
    }
    private void UpdateCollider(int indexMesh, int listColliderIndex)
    {
        if (mesh.vertexCount <= indexMesh + 6)
            return;

        BoxCollider boxCollider = splineCollider[listColliderIndex];
        Bounds newBounds = new Bounds();

        newBounds.min = mesh.vertices[indexMesh];
        newBounds.max = mesh.vertices[indexMesh + 6];

        boxCollider.isTrigger = false;

        if (IsBoundsEqual(newBounds, boxCollider.bounds))
            return;

        boxCollider.center = newBounds.center;
        boxCollider.size = newBounds.size.Abs();
    }
    private void DestroyCollider(int listIndex)
    {
        Destroy(splineCollider[listIndex]);
        splineCollider.RemoveAt(listIndex);
    }
    private bool IsBoundsEqual(Bounds first, Bounds second)
    {
        if (first.min != second.min)
            return false;

        if (first.max != second.max)
            return false;

        if (first.size != second.size)
            return false;

        if (first.center != second.center)
            return false;

        return true;
    }
    private void DestroyAllColliders()
    {
        int count = splineCollider.Count;
        for (int i = 0; i < count; i++)
        {
            DestroyCollider(0);
        }
    }
    private void TriggerCollision(int listIndex) => splineCollider[listIndex].isTrigger = true;
}