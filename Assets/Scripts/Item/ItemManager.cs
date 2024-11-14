using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    #region Singleton
    private static ItemManager instance;
    public static ItemManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
    #endregion

    private float distPlayer = 15f;
    private List<GPUItem> gpuItems = new List<GPUItem>();

    public void DropItem(SOItems item, int value, Vector3 position, Quaternion rotation)
    {
        Vector3 itemPosition = position + item.poseOffset.position;
        Quaternion itemRotation = rotation * item.poseOffset.rotation;

        gpuItems.Add(new GPUItem(item, value, itemPosition, itemRotation));
    }

    private void Update()
    {
        for (int i = 0; i < gpuItems.Count; i++)
        {
            GPUItem gpuItem = gpuItems[i];
            Vector3 gpuItemPosition = gpuItem.transform.GetPosition();
            if (!gpuItem.alreadyGenerateCollision && PlayerTooClose(gpuItemPosition))
            {
                GenerateCollision(gpuItem);
            }
            else if (gpuItem.alreadyGenerateCollision)
            {
                if (PlayerTooClose(gpuItemPosition))
                {
                    UpdateCollider(gpuItem);
                }
                else
                {
                    DeleteCollider(gpuItem);
                }
            }

            Graphics.RenderMesh(new RenderParams(gpuItem.item.material), gpuItem.item.mesh, 0, gpuItem.transform);
        }
    }

    private bool PlayerTooClose(Vector3 position)
    {
        return Vector3.Distance(Player.Instance.GetPosition(), position) < distPlayer;
    }

    private void GenerateCollision(GPUItem gpuItem)
    {
        GameObject itemWorld = new GameObject();
        ItemInWorld itemInWorld = itemWorld.AddComponent<ItemInWorld>();

        itemInWorld.Init(gpuItem.item, gpuItem.value);
        itemInWorld.UpdateTransform(gpuItem.transform.GetPosition(), gpuItem.transform.rotation);

        gpuItem.itemInWorld = itemInWorld;
        gpuItem.alreadyGenerateCollision = true;
    }

    private void UpdateCollider(GPUItem gpuItem)
    {
        if (gpuItem.itemInWorld == null)
        {
            gpuItems.Remove(gpuItem);
            return;
        }

        gpuItem.itemInWorld.UpdateTransform(gpuItem.transform.GetPosition(), gpuItem.transform.rotation);
    }

    private void DeleteCollider(GPUItem gpuItem)
    {
        gpuItem.itemInWorld.Destroy();
        gpuItem.itemInWorld = null;
        gpuItem.alreadyGenerateCollision = false;
    }
}