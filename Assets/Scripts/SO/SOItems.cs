using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item", order = 1)]
public class SOItems : ScriptableObject
{
    [Header("World")]
    public Mesh mesh;
    public Material material;
    public GameObject gameObject;

    [Header("UI")]
    [SerializeField] private Sprite spriteItem;
    [SerializeField] private string nameItem;
    public string description;
    public int id;
    public Sprite Sprite { get => spriteItem; set => spriteItem = value; }
    public string NameItem { get => nameItem; set => nameItem = value; }

    [Header("Data")]
    [SerializeField] private int maxStack;
    [SerializeField] private bool isDiscover;
    public bool isEquipable;
    public ItemType itemType = ItemType.All;

    [Header("Offset")]
    public Pose poseOffset = new Pose(Vector3.zero, Quaternion.identity);

    [Header("Audio")]
    public AudioClip clip;

    public int MaxStack { get => maxStack; set => maxStack = value; }
    public bool IsDiscover
    {
        get => isDiscover;
        set
        {
            isDiscover = value;
        }
    }
}

public enum ItemType
{
    All,
    Resource,
    World,
    Mob,
    Weapon,
}