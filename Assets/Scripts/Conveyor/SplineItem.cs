using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineItem : MonoBehaviour
{
    private List<ItemOnConvey> itemsOnConvey = new List<ItemOnConvey>();
    private Spline spline;

    private const float itemSpeed = 20f;
    private const float playerMinDist = 15f;

    private float distBetweenItem = 0.05f;
    private float stepConvey = 0f;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        spline = GetComponent<SplineContainer>().Spline;
    }
    public void SetAllItems(List<ItemOnConvey> items) => itemsOnConvey = items;
    private void Update()
    {
        MoveItemsOnConvey();
        DrawItems();
    }

    public void ClearItems()
    {
        for (int i = 0; i < itemsOnConvey.Count; i++)
        {
            if (itemsOnConvey[i].alreadyGenerateCollision)
            {
                itemsOnConvey[i].itemInWorld.Destroy();
            }
        }
        itemsOnConvey.Clear();
    }
    public List<ItemOnConvey> GetAllItems()
    {
        return itemsOnConvey;
    }
    public void UpdateStep()
    {
        Init();
        stepConvey = 0.1f / spline.GetLength();
        distBetweenItem = stepConvey * 20f;
    }
    public bool CanAddItemOnConvey()
    {
        if (itemsOnConvey.Count == 0)
        {
            return true;
        }

        if (0f + distBetweenItem >= itemsOnConvey[itemsOnConvey.Count - 1].time)
        {
            return false;
        }

        return true;
    }
    public void AddItemOnConvey(SOItems item)
    {
        ItemOnConvey newItem = new ItemOnConvey(0f, itemSpeed, true, item);
        itemsOnConvey.Add(newItem);
        UpdateItemOnConveyTransform(newItem);
    }
    public SOItems GetItemOnConvey()
    {
        if (itemsOnConvey.Count == 0)
        {
            return null;
        }

        ItemOnConvey itemConvey = itemsOnConvey[0];
        if (itemConvey.time >= 0.92f)
        {
            SOItems item = itemConvey.item;
            if (itemConvey.alreadyGenerateCollision)
            {
                itemConvey.itemInWorld.Destroy();
            }
            itemsOnConvey.RemoveAt(0);
            return item;
        }

        return null;
    }
    public bool IsSameItemOnConvey(SOItems item)
    {
        if (itemsOnConvey.Count == 0)
        {
            return false;
        }

        return itemsOnConvey[0].item == item;
    }
    public bool CanGetItemOnConvey()
    {
        if (itemsOnConvey.Count == 0)
        {
            return false;
        }

        if (itemsOnConvey[0].time >= 1f)
        {
            return true;
        }

        return false;
    }

    private void MoveItemsOnConvey()
    {
        for (int i = 0; i < itemsOnConvey.Count; i++)
        {
            ItemOnConvey item = itemsOnConvey[i];
            if (item.time < 1f)
            {
                if (item.isMoving)
                {
                    item.time += stepConvey * TimeScale.deltaTime * itemSpeed;

                    if (item.time >= 1f)
                    {
                        item.time = 1f;
                    }
                }

                item.isMoving = !IsCollidingAnotherItem(i);
                UpdateItemOnConveyTransform(item);
            }
        }
    }
    private void UpdateItemOnConveyTransform(ItemOnConvey itemOnConvey)
    {
        SplineUtilities.Evaluate(spline, itemOnConvey.time, out Vector3 position, out Vector3 forward, out Vector3 right, out Quaternion rotation);

        Vector3 itemOffset = itemOnConvey.item.poseOffset.position;
        Vector3 worldPositionOffset = (itemOffset.x * right) + new Vector3(0f, itemOffset.y, 0f) + (itemOffset.z * forward);
        Vector3 itemPosition = transform.position + position + worldPositionOffset;

        Quaternion rotationOffset = itemOnConvey.item.poseOffset.rotation;
        Quaternion worldRotationOffset = transform.rotation * rotationOffset;
        Quaternion itemRotation = rotation * worldRotationOffset;

        itemOnConvey.transform.SetTRS(itemPosition, itemRotation, Vector3.one);
    }
    private bool IsCollidingAnotherItem(int index)
    {
        if (index == 0)
        {
            return false;
        }

        if (itemsOnConvey[index].time + distBetweenItem >= itemsOnConvey[index - 1].time)
        {
            return true;
        }

        return false;
    }

    private void DrawItems()
    {
        for (int i = 0; i < itemsOnConvey.Count; i++)
        {
            ItemOnConvey itemOnConvey = itemsOnConvey[i];

            if (!itemOnConvey.alreadyGenerateCollision && TooClosePlayer(i))
            {
                GenerateCollision(ref itemOnConvey);
            }
            else if (itemOnConvey.alreadyGenerateCollision)
            {
                if (TooClosePlayer(i))
                {
                    UpdateCollider(i);
                }
                else
                {
                    DeleteCollider(i);
                }
            }

            if (!itemOnConvey.alreadyGenerateCollision)
            {
                Graphics.RenderMesh(new RenderParams(itemOnConvey.item.material), itemOnConvey.item.mesh, 0, itemOnConvey.transform);
            }
        }
    }
    private bool TooClosePlayer(int index)
    {
        return Vector3.Distance(Player.Instance.gameObject.transform.position, itemsOnConvey[index].transform.GetPosition()) < playerMinDist;
    }
    private void GenerateCollision(ref ItemOnConvey itemOnConvey)
    {
        GameObject itemWorld = new GameObject();
        ItemInWorld itemInWorld = itemWorld.AddComponent<ItemInWorld>();

        itemInWorld.Init(itemOnConvey.item, 1);
        itemInWorld.UpdateTransform(itemOnConvey.transform.GetPosition(), itemOnConvey.transform.rotation);

        itemOnConvey.itemInWorld = itemInWorld;
        itemOnConvey.alreadyGenerateCollision = true;
    }
    private void UpdateCollider(int index)
    {
        if (itemsOnConvey[index].itemInWorld == null)
        {
            itemsOnConvey.RemoveAt(index);
            return;
        }
        itemsOnConvey[index].itemInWorld.UpdateTransform(itemsOnConvey[index].transform.GetPosition(), itemsOnConvey[index].transform.rotation);
    }
    private void DeleteCollider(int index)
    {
        itemsOnConvey[index].itemInWorld.Destroy();
        itemsOnConvey[index].itemInWorld = null;
        itemsOnConvey[index].alreadyGenerateCollision = false;
    }
}