using UnityEngine;

[System.Serializable]
public class ItemOnConvey : GPUItem
{
    public float time;
    public bool isMoving;

    public ItemOnConvey() { }

    public ItemOnConvey(float time, float speed, bool isMoving, SOItems item)
    {
        this.time = time;
        this.isMoving = isMoving;
        this.item = item;
    }
}

[System.Serializable]
public class GPUItem
{
    public SOItems item;
    public int value = 1;

    public Matrix4x4 transform;

    public bool alreadyGenerateCollision = false;
    public ItemInWorld itemInWorld = null;

    public GPUItem() { }
    public GPUItem(SOItems item, Vector3 position, Quaternion rotation)
    {
        this.item = item;
        transform.SetTRS(position, rotation, Vector3.one);
        value = 1;
    }
    public GPUItem(SOItems item, int value, Vector3 position, Quaternion rotation)
    {
        this.item = item;
        transform.SetTRS(position, rotation, Vector3.one);
        this.value = value;
    }
}