using UnityEngine;

public class ItemInWorld : MonoBehaviour, ISelectable
{
    private SOItems item;
    private int value;
    private bool isDestroyed;
    public void Init(SOItems item, int value)
    {
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshCollider>();

        this.item = item;
        this.value = value;
        name = item.name;

        GetComponent<MeshFilter>().mesh = item.mesh;
        GetComponent<MeshRenderer>().material = item.material;

        MeshCollider mCol = GetComponent<MeshCollider>();
        mCol.sharedMesh = item.mesh;
        mCol.convex = true;
        mCol.isTrigger = true;

        gameObject.layer = 6;
    }

    public void Deselect()
    {
        if (!isDestroyed)
        {
            gameObject.layer = 6;
        }
    }

    public void Interact()
    {
        if (RessourcesRayManager.instance.LastSelected != null && this != RessourcesRayManager.instance.LastSelected as ItemInWorld)
        {
            RessourcesRayManager.instance.OnDeselectInvoke();
        }

        RessourcesRayManager.instance.LastSelected = this;
        Select();


        RessourcesRayManager.instance.InteractionMessage.SetRessourceTextEnable(item.name);
    }

    public void Select()
    {
        gameObject.layer = 8;
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void Destroy()
    {
        isDestroyed = true;
        RessourcesRayManager.instance.OnDeselectInvoke();
        Destroy(gameObject);
    }

    public void InteractItem()
    {
        Player.Instance.AddItem(item, value);
        int itemAmount = Player.Instance.GetItemAmount(item);
        RessourcesRayManager.instance.RessourcesInfos.SetText("+ " + value + " " + item.NameItem + " (" + itemAmount + ")");
        RessourcesRayManager.instance.RessourcesInfos.EnableMessage();
        Destroy();
    }
}