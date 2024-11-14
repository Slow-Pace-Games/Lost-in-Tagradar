using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyDistributor : Building, ISelectable
{
    [Header("Animation")]
    [SerializeField] protected List<MeshRenderer> animationsMesh;

    [Header("Electricity")]
    public List<EnergyDistributor> adjacentDistributors = new List<EnergyDistributor>();
    public List<Machine> linkedMachine = new List<Machine>();
    public List<Generator> generators = new List<Generator>();
    public bool isGenerator;
    public bool isInit;
    public Animator anim;
    [SerializeField] private Transform[] select;
    public List<GameObject> electricBalls;

    [SerializeField] private string type;
    public string Type { get { return type; } }


    [Header("Collision")]
    [SerializeField] protected SphereCollider sphereCollider;

    protected override void ActivateMesh(bool activate)
    {
        base.ActivateMesh(activate);
        foreach (MeshRenderer mesh in animationsMesh)
        {
            mesh.enabled = activate;
        }
        anim.SetBool("Deploy", activate);
    }

    protected void PutHoloOnAnim(Material holo)
    {
        foreach (MeshRenderer mesh in animationsMesh)
        {
            mesh.material = holo;
        }
    }

    protected void PutMatOnAnimation()
    {
        for (int i = 0; i < animationsMesh.Count(); ++i)
        {
            if (i == 0)
            {
                animationsMesh[i].material = materialPreview[2];
            }
            else if (i == 1)
            {
                animationsMesh[i].material = materialPreview[3];
            }
            else
            {
                animationsMesh[i].material = materialPreview[4];
            }
        }
    }

    private void Start()
    {
        linkedMachine = new List<Machine>();
    }

    private void Update()
    {
        if (isPlaced)
        {
            if (instantiatedEffect != null)
            {
                effectTimer += Time.deltaTime;
                if (effectTimer >= effectMaxTimer)
                {
                    Destroy(instantiatedEffect.gameObject);
                    ActivateMesh(true);
                }
            }
        }
    }

    #region NodeManager

    public void UpdateNode(EnergyDistributor lastNode)
    {
        foreach (EnergyDistributor node in adjacentDistributors)
        {
            foreach (Generator generator in generators)
            {
                if (node != lastNode && !node.generators.Contains(generator))
                {
                    node.generators.Add(generator);
                }
            }
        }
    }

    #endregion

    #region IBuildable

    public override void LoadBuild(List<int> outputIDs = null, List<int> inputIDs = null, List<int> outputStacks = null, List<int> inputStacks = null)
    {
        base.LoadBuild(outputIDs, inputIDs, outputStacks, inputStacks);
        PutMatOnAnimation();
        ElecBallsManager.Instance.AddDistributor(this);
    }

    public override void Build()
    {
        base.Build();
        PlaySoundPuller(audioSource1, buildingData.sounds.GetAllClips(MachineAudio.Ambiance), 10);
        sphereCollider.enabled = true;
        foreach (GameObject go in electricBalls)
        {
            go.SetActive(false);
        }
        PutMatOnAnimation();
        ElecBallsManager.Instance.AddDistributor(this);
    }

    public override void Destruct()
    {
        foreach (Machine machine in linkedMachine)
        {
            machine.NearbyDistributors.Remove(this);
        }

        foreach (EnergyDistributor distributor in adjacentDistributors)
        {
            distributor.adjacentDistributors.Remove(this);
            if (isGenerator)
            {
                distributor.generators.Remove(this as Generator);
            }
        }

        if (this is Generator)
        {
            Destroy(this);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
        base.Destruct();
        ElecBallsManager.Instance.DeleteDistributor(this);
    }

    public override bool CanBeBuilt(bool isSnapped)
    {
        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            if (Player.Instance.GetItemAmount(buildingData.costs[i].item) < buildingData.costs[i].value) //Check in inventory if the component are there in the adequate quantity
            {
                meshRenderer.material = materialPreview[1];
                PutHoloOnAnim(materialPreview[1]);
                return false;
            }
        }

        if (collisionCounter == 0 && isSnapped)
        {
            meshRenderer.material = materialPreview[0];
            PutHoloOnAnim(materialPreview[0]);
            return true;
        }

        meshRenderer.material = materialPreview[1];
        PutHoloOnAnim(materialPreview[1]);
        return false;
    }

    #endregion

    #region ISelectable
    public void Select()
    {
        for (int i = 0; i < select.Length; i++)
        {
            select[i].gameObject.layer = 8;
        }
    }

    public void Deselect()
    {
        for (int i = 0; i < select.Length; i++)
        {
            select[i].gameObject.layer = 0;
        }
    }

    public void Interact()
    {
        if (Player.Instance.GetOnDestructionMode())
        {
            if (RessourcesRayManager.instance.LastSelected != null && this != RessourcesRayManager.instance.LastSelected as EnergyDistributor)
            {
                RessourcesRayManager.instance.OnDeselectInvoke();
            }

            RessourcesRayManager.instance.LastSelected = GetComponent<ISelectable>();
            Select();

            RessourcesRayManager.instance.InteractionMessage.SetDestructTextEnable("Pylon");
        }
    }
    #endregion

}