using UnityEngine;

public class AnimateConvey : MonoBehaviour, IDestructible, ISelectable
{
    [Header("Parameter")]
    [SerializeField, Range(0.001f, 5f)] private float speed = 0.01f;
    private MeshFilter meshFilter;
    private Rigidbody rigidbodyTemp;
    private bool canMoveUvs = false;
    private bool isCollide = false;
    private Vector2[] uv;
    private bool isDestroy = false;

    public bool IsCollide { get => isCollide; }

    public void Init()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        rigidbodyTemp = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (canMoveUvs)
        {
            MoveUvs();
        }
    }

    public void SetBuild()
    {
        if (rigidbodyTemp != null)
        {
            rigidbodyTemp.isKinematic = true;
            Destroy(rigidbodyTemp);
            rigidbodyTemp = null;
        }

        canMoveUvs = true;

        CenterUvs();
    }

    private void CenterUvs()
    {
        Vector3[] vertices = meshFilter.sharedMesh.vertices;

        if (vertices.Length == 0)
        {
            return;
        }

        Vector2[] newUvs = new Vector2[vertices.Length];

        float uvOffset = 0;
        for (int i = 2; i < vertices.Length; i += 4)
        {
            Vector3 p1 = vertices[i];
            Vector3 p3;

            if (i + 4 >= vertices.Length)
            {
                p3 = vertices[2];
            }
            else
            {
                p3 = vertices[i + 4];
            }

            float distance = Vector3.Distance(p1, p3);
            float uvDistance = distance + uvOffset;

            newUvs[i] = new Vector2(0, uvOffset);
            newUvs[i + 1] = new Vector2(1, uvOffset);

            if (i + 4 <= vertices.Length)
            {
                newUvs[i + 4] = new Vector2(1, uvDistance);
                newUvs[i + 5] = new Vector2(0, uvDistance);
            }

            uvOffset += distance;
        }

        meshFilter.sharedMesh.uv = newUvs;
        uv = newUvs;
    }
    private void MoveUvs()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i].y -= speed * TimeScale.deltaTime;
            }

            meshFilter.sharedMesh.SetUVs(0, uv);
        }
    }

    #region IDestructible
    public void Destruct()
    {
        GetComponentInParent<Convey>().Destruct();
    }
    #endregion

    private void OnCollisionEnter(Collision other) => isCollide = true;
    private void OnCollisionStay(Collision other) => isCollide = true;
    private void OnCollisionExit(Collision other) => isCollide = false;

    #region ISelectable
    public void Select()
    {
        gameObject.layer = 8;
    }

    public void Deselect()
    {
        if (isDestroy)
            return;

        gameObject.layer = 0;
    }

    public void Interact()
    {
        if (Player.Instance.GetOnDestructionMode())
        {
            if (RessourcesRayManager.instance.LastSelected != null && this != RessourcesRayManager.instance.LastSelected as AnimateConvey)
            {
                RessourcesRayManager.instance.OnDeselectInvoke();
            }

            RessourcesRayManager.instance.LastSelected = this;
            Select();

            RessourcesRayManager.instance.InteractionMessage.SetDestructTextEnable("Convey");
        }
    }
    #endregion

    #region IDestructible
    public void Destruction() => isDestroy = true;
    #endregion
}