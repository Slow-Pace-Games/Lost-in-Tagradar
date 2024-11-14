using System.Collections.Generic;
using UnityEngine;

public class Generator : EnergyDistributor, IMachineInteractable, IColliderIO, IAttachement, ISelectable
{
    [Header("Input/Output")]
    [SerializeField] Collider inputCollider;
    [SerializeField] List<Attachement> input;

    [Header("Data")]
    [SerializeField] public float electricity = 500;
    bool isProducing = false;
    float timerMax = 0.0f;
    float timer = 0.0f;
    bool windowIsOpen = false;
    public float maxElectricity = 0;
    public float usedElectricity = 0;

    [Header("Fuel Stack")]
    [SerializeField] protected List<SOItems> itemInputType;
    [SerializeField] protected List<int> itemInputStack;

    [Header("Debug")]
    [SerializeField] private bool debug;

    [Header("Transition Recipes")]
    [SerializeField] private SOTransition[] transitions;
    [SerializeField] public SOTransition currentTransition;
    [SerializeField] private List<SOItems> itemAccepted;

    private RessourcesRayManager ressourcesRayManager = RessourcesRayManager.instance;

    public List<int> ItemInputStack { get => itemInputStack; }

    private void SendDataToHUD()
    {
        if (windowIsOpen)
        {
            MachineHUDManager.Instance.InputStackChange(itemInputStack);
            MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
        }
    }

    private void InitGenerator()
    {
        itemAccepted = new List<SOItems>();
        foreach (SOTransition transition in transitions)
        {
            itemAccepted.Add(transition.Resource);
        }
    }

    public override void LoadBuild(List<int> outputIDs, List<int> inputIDs, List<int> outputStacks, List<int> inputStacks)
    {
        base.LoadBuild(outputIDs, inputIDs, outputStacks, inputStacks);
        InitGenerator();
    }

    public override void Build()
    {
        base.Build();
        InitGenerator();
        Loid.Instance.UpdateTuto(PlayerAction.PlaceGenerator);
    }

    private void Update()
    {
        if (IsPlaced || debug)
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

            GetItemOnSpline();
            BurnFuel();
        }
    }

    public void ChangeCurrentTransition(SOItems item)
    {
        foreach (SOTransition transition in transitions)
        {
            if (transition.Resource == item)
            {
                currentTransition = transition;
                itemInputType[0] = item;
                timerMax = currentTransition.Timer;
                timer = 0;
                MachineHUDManager.Instance.onUpdateUIGenerator?.Invoke(currentTransition);
            }
        }
    }

    private bool IsFuel(SOItems fuel)
    {
        return itemAccepted.Contains(fuel);
    }

    private void BurnFuel()
    {
        if (itemInputStack[0] > 0 && IsFuel(itemInputType[0]) && !isProducing)
        {
            ChangeCurrentTransition(itemInputType[0]);
            isProducing = true;
            itemInputStack[0]--;
            maxElectricity = electricity;
            SendDataToHUD();
        }

        if (isProducing)
        {
            timer += Time.deltaTime;
            if (timer >= timerMax)
            {
                timer = 0;
                if (itemInputStack[0] > 0 && IsFuel(itemInputType[0]))
                {
                    itemInputStack[0]--;
                    ChangeCurrentTransition(itemInputType[0]);
                    PlaySound(audioSource2, buildingData.sounds.GetRandomClip(MachineAudio.Work), false);
                }
                else
                {
                    ChangeCurrentTransition(itemInputType[0]);
                    isProducing = false;
                    maxElectricity = 0;
                }
                SendDataToHUD();
            }
        }
    }

    private void GetItemOnSpline()
    {
        if (itemInputStack[0] < 100 && input[0].Convey != null)
        {
            if (itemInputStack[0] == 0)
            {
                foreach (SOItems fuel in itemAccepted)
                {
                    if (input[0].Convey.CanGetItemOnConvey() && input[0].Convey.IsSameItemOnConvey(fuel))
                    {
                        itemInputType[0] = input[0].Convey.GetItemOnConvey();
                        ChangeCurrentTransition(itemInputType[0]);
                        itemInputStack[0]++;
                        SendDataToHUD();
                    }
                }
            }
            else if (input[0].Convey.CanGetItemOnConvey() && input[0].Convey.IsSameItemOnConvey(itemInputType[0]))
            {
                itemInputType[0] = input[0].Convey.GetItemOnConvey();
                itemInputStack[0]++;
                SendDataToHUD();
            }
        }
    }

    #region IColliderIO

    public void ActivateIO()
    {
        inputCollider.enabled = true;
    }

    public void DeactivateIO()
    {
        inputCollider.enabled = false;
    }

    #endregion

    #region IMachineInteractable

    public void OpenWindow()
    {
        MachineHUDManager.Instance.OpenGeneratorMenu(currentTransition, timer, timerMax, isProducing, itemAccepted);
        windowIsOpen = true;
        MachineHUDManager.Instance.InputStackChange(itemInputStack);
        MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
        MachineHUDManager.Instance.onStackValueChange += AddStack;
        MachineHUDManager.Instance.onTransitionChange += ChangeCurrentTransition;
    }

    public void CloseWindow()
    {
        MachineHUDManager.Instance.CloseGeneratorMenu();
        windowIsOpen = false;
        MachineHUDManager.Instance.onStackValueChange -= AddStack;
        MachineHUDManager.Instance.onTransitionChange -= ChangeCurrentTransition;
    }

    #endregion
    private void AddStack(int nbStackToAdd, bool isInInput, int index, SOItems itemType)
    {
        itemInputStack[index] += nbStackToAdd;
        itemInputType[index] = itemType;
    }

    public List<Attachement> GetInputAttachements()
    {
        return input;
    }

    public List<Attachement> GetOutputAttachements()
    {
        return null;
    }

    #region ISelectable
    public new void Select()
    {
        meshRenderer.gameObject.layer = 8;
    }
    public new void Deselect()
    {
        if (isPlaced)
            meshRenderer.gameObject.layer = 0;
    }
    public new void Interact()
    {
        if (IsPlaced)
        {
            if (ressourcesRayManager.LastSelected != null && this != ressourcesRayManager.LastSelected as Generator)
            {
                ressourcesRayManager.LastSelected.Deselect();
            }

            ressourcesRayManager.LastSelected = this;
            Select();

            ressourcesRayManager.InteractionMessage.SetMachineTextEnable(Type);
        }
    }
    #endregion

    public List<int> GetIdsItemInput()
    {
        List<int> ids = new List<int>();
        foreach (SOItems item in itemInputType)
        {
            ids.Add(item.id);
        }
        return ids;
    }
}
