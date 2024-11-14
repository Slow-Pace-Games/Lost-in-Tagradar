using UnityEngine;

public class Attachement : MonoBehaviour
{
    public enum DirectionIO
    {
        Output,
        Input,
    }

    [Header("Parameter")]
    [SerializeField] private DirectionIO direction;
    [SerializeField] private Transform transformIO;
    [SerializeField] private Convey convey;
    [SerializeField] private bool isFixed = false;
    [SerializeField] private bool isAConveyor = false;
    private Collider colliderIO;

    public DirectionIO Direction { get => direction; }
    public Transform TransformIO { get => transformIO; }
    public Convey Convey { get => convey; set => convey = value; }
    public bool IsFixed { get => isFixed; set => isFixed = value; }
    public bool IsAConveyor { get => isAConveyor; }

    private void Start()
    {
        this.colliderIO = GetComponent<Collider>();
    }

    public void Init(DirectionIO direction, bool isAConvey)
    {
        this.direction = direction;
        if (this.direction == DirectionIO.Input)
            transformIO.localRotation = Quaternion.Euler(0f, 180f, 0f);
        this.isAConveyor = isAConvey;
        this.colliderIO = GetComponent<Collider>();
    }

    public void DesactivateCollider()
    {
        colliderIO.enabled = false;
    }

    public void ReactivateCollider()
    {
        colliderIO.enabled = true;
    }
}