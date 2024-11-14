using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//pense a ref tes component dans ta class sa vaudra mieux que a chaque fois que tu collisonne tu get le collider et pour les io desactive les au début puis active les et ensuite
//si une spline se connecte les desac sa peux se penser pour opti un peux 

public class Machine : Building, IBuildable, IColliderIO
{
    [Header("Data")]

    [Header("Input/Output")]
    [SerializeField] protected Collider[] inputOutput;

    [Header("Machine Param")]
    [SerializeField] protected MachineRank tier;
    [SerializeField] protected float timerMax = 5f;
    [SerializeField] protected float timer = 0f;
    public float Timer { get => timer; }

    [Header("Machine Stack")]
    [SerializeField] protected List<SOItems> itemsOutputType;
    [SerializeField] protected List<SOItems> itemsInputType;
    [SerializeField] protected List<int> itemsOutputStack;
    [SerializeField] protected List<int> itemsInputStack;

    [Header("Electricity")]
    [SerializeField] protected List<EnergyDistributor> nearbyDistributors;
    public EnergyDistributor AddNearbyDistributors { set { if (!nearbyDistributors.Contains(value)) { nearbyDistributors.Add(value); } } }
    public List<EnergyDistributor> NearbyDistributors { get => nearbyDistributors; }

    [Header("UI")]
    [SerializeField] protected bool windowIsOpen = false;
    public bool WindowIsOpen { get => windowIsOpen; }

    public List<int> ItemsOutputStack { get => itemsOutputStack; }
    public List<int> ItemsInputStack { get => itemsInputStack; }

    private void Start()
    {
        if (particle != null)
        {
            var main = particle.main;
            main.maxParticles = 0;
        }
    }

    #region IBuildable

    public void InitMachine(List<int> outputIDs, List<int> inputIDs, List<int> outputStacks, List<int> inputStacks)
    {
        SetItemInput(inputIDs);
        SetItemOutput(outputIDs);
        itemsOutputStack = outputStacks;
        itemsInputStack = inputStacks;
    }

    public override void LoadBuild(List<int> outputIDs, List<int> inputIDs, List<int> outputStacks, List<int> inputStacks)
    {
        for (int i = 0; i < inputOutput.Length; i++)
        {
            inputOutput[i].enabled = true;
        }
        gameObject.GetComponent<NavMeshObstacle>().enabled = true;

        meshRenderer.material = materialPreview[2];
        IsPlaced = true;

        instantiatedEffect = Instantiate(effect, transform.position, transform.rotation);
        instantiatedEffect.Play();

        ActivateMesh(false);

        SetArrowActive(false);
        InitMachine(outputIDs, inputIDs, outputStacks, inputStacks);
    }

    public override void Build()
    {
        for (int i = 0; i < inputOutput.Length; i++)
        {
            inputOutput[i].enabled = true;
        }
        base.Build();
    }
    #endregion

    public int GetMachineTier()
    {
        return (int)tier;
    }

    public void SetMachineTier(int tier)
    {
        this.tier = (MachineRank)tier;
    }

    #region IColliderIO

    public void ActivateIO()
    {
        foreach (Collider collider in inputOutput)
        {
            collider.enabled = true;
        }
    }

    public void DeactivateIO()
    {
        foreach (Collider collider in inputOutput)
        {
            collider.enabled = false; ;
        }
    }

    #endregion

    public List<int> GetIdsItemInput()
    {
        List<int> ids = new List<int>();
        foreach (SOItems item in itemsInputType)
        {
            if (item != null)
            {
                ids.Add(item.id);
            }
            else
            {
                ids.Add(-1);
            }
        }
        return ids;
    }

    public List<int> GetIdsItemOutput()
    {
        List<int> ids = new List<int>();
        foreach (SOItems item in itemsOutputType)
        {
            if (item != null)
            {
                ids.Add(item.id);
            }
            else
            {
                ids.Add(-1);
            }
        }
        return ids;
    }

    private void SetItemInput(List<int> itemInputIDs)
    {
        if (itemInputIDs.Count > 0)
        {
            for (int i = 0; i < itemInputIDs.Count; i++)
            {
                if (itemInputIDs[i] != -1)
                {
                    itemsInputType[i] = database.AllItems[itemInputIDs[i]];
                }
            }
        }
    }

    private void SetItemOutput(List<int> itemOutputIDs)
    {
        if (itemOutputIDs.Count > 0)
        {
            for (int i = 0; i < itemOutputIDs.Count; i++)
            {
                if (itemOutputIDs[i] != -1)
                {
                    itemsOutputType[i] = database.AllItems[itemOutputIDs[i]];
                }
            }
        }
    }
}