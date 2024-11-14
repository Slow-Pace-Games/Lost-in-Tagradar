using System.Collections.Generic;
using UnityEngine;

public class Drill : Machine, IMachineInteractable, IAttachement, ISelectable
{
    [Header("Visual Effects")]
    [SerializeField] List<ParticleSystem> particles;

    [Header("Input/Output")]
    [SerializeField] List<Attachement> output;

    [Header("Debug")]
    [SerializeField] private bool debug;

    [Header("Data")]
    [SerializeField] private SOTransition[] transitions;
    [SerializeField] public SOTransition currentTransition;
    public SORessource resource;
    bool isProducing = false;
    [SerializeField] private string type;
    float lastTransitionCost = 0;
    public string Type { get { return type; } }

    private RessourcesRayManager ressourcesRayManager = RessourcesRayManager.instance;

    private void Start()
    {
        collisionCounter = 1;
    }

    public void InitDrill()
    {
        Vector3 vec;
        vec = new Vector3(0, -1, 0);
        Vector3 vec1;
        vec1 = new Vector3(0, 5, 0);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + vec1, vec, out hit, 100, 1 << LayerMask.NameToLayer("Deposit")))
        {
            resource = hit.transform.gameObject.GetComponent<RessourceExploitable>().sORessource;
        }

        foreach (SOTransition transition in transitions)
        {
            if (transition.Resource == resource.item)
            {
                currentTransition = transition;
                itemsOutputType[0] = currentTransition.Resource;
                timerMax = currentTransition.Timer;
                timer = 0;
            }
        }

        isProducing = false;
    }

    private void ChangeEmissiveColor(float red, float green, float blue)
    {
        var emmisiveColor = meshRenderer.material.GetColor("_Emissive_Color");
        emmisiveColor.r = red;
        emmisiveColor.g = green;
        emmisiveColor.b = blue;
        meshRenderer.material.SetColor("_Emissive_Color", emmisiveColor);
    }

    private void SendDataToHUD()
    {
        if (WindowIsOpen)
        {
            MachineHUDManager.Instance.OutputStackChange(itemsOutputStack);
            MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
        }
    }

    private void UseElectricity()
    {
        float elecToChange = currentTransition.ElectricityCost;
        elecToChange -= lastTransitionCost;

        if (isAlimented)
        {
            foreach (EnergyDistributor distributor in nearbyDistributors)
            {
                if (distributor.isGenerator)
                {
                    if ((distributor as Generator).usedElectricity + elecToChange <= (distributor as Generator).maxElectricity && (distributor as Generator).maxElectricity > 0)
                    {
                        isProducing = true;
                        SendDataToHUD();
                        return;
                    }
                }
                foreach (Generator generator in distributor.generators)
                {
                    if (generator.usedElectricity + elecToChange <= generator.maxElectricity && generator.maxElectricity > 0)
                    {
                        isProducing = true;
                        SendDataToHUD();
                        return;
                    }
                }
            }

            StopSound(audioSource1);
            isAlimented = false;
            MachineHUDManager.Instance.IsMachineAlimentedChange(isAlimented);
            isProducing = false;
            ChangeEmissiveColor(255, 0, 0);
            if (particle != null)
            {
                var main = particle.main;
                main.maxParticles = 0;
            }
            SendDataToHUD();
            lastTransitionCost = 0;
        }
        else
        {
            if (nearbyDistributors.Count == 0)
            {
                ChangeEmissiveColor(255, 0, 0);
            }

            foreach (EnergyDistributor distributor in nearbyDistributors)
            {
                if (distributor.isGenerator)
                {
                    CheckElec(distributor as Generator, elecToChange);
                    return;
                }
                foreach (Generator generator in distributor.generators)
                {
                    if (CheckElec(generator, elecToChange, distributor))
                    {
                        return;
                    }
                }
            }
        }
    }

    private bool CheckElec(Generator generator, float elecToChange, EnergyDistributor distributor = null)
    {
        if (generator.usedElectricity + elecToChange <= generator.maxElectricity && generator.maxElectricity > 0)
        {
            isAlimented = true;
            MachineHUDManager.Instance.IsMachineAlimentedChange(isAlimented);
            PlaySoundPuller(audioSource1, buildingData.sounds.GetAllClips(MachineAudio.Ambiance), 5);
            if (distributor != null)
            {
                distributor.UpdateNode(distributor);
            }
            else
            {
                generator.UpdateNode(generator);
            }
            lastTransitionCost = currentTransition.ElectricityCost;
            ChangeEmissiveColor(0, 255, 0);
            SendDataToHUD();
            return true;
        }
        return false;
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

            UseElectricity();

            if (isAlimented)
            {
                if (itemsOutputStack[0] < 100)
                {
                    timer += Time.deltaTime;
                }

                if (timer >= timerMax)
                {
                    PlaySound(audioSource2, buildingData.sounds.GetRandomClip(MachineAudio.Work), false);
                    itemsOutputStack[0]++;
                    timer = 0.0f;
                    SendDataToHUD();
                }
                //PlaySound(); TODO
            }
        }


        if (output[0].Convey != null && itemsOutputStack[0] > 0 && isAlimented)
        {
            if (output[0].Convey.CanAddItemOnConvey())
            {
                output[0].Convey.AddItemOnConvey(resource.item);
                itemsOutputStack[0]--;
                PlaySound(audioSource2, buildingData.sounds.GetRandomClip(MachineAudio.Finish), false);
                SendDataToHUD();
            }
        }
    }

    #region collision

    public override void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Deposit"))
        {
            collisionCounter++;
        }
        else
        {
            collisionCounter--;
        }
    }

    public override void OnTriggerExit(Collider collider)
    {
        HealthComponent garbage;
        if (collider.gameObject.TryGetComponent<HealthComponent>(out garbage))
        {
            BoxCollider bCollider;
            if (gameObject.TryGetComponent<BoxCollider>(out bCollider))
            {
                bCollider.isTrigger = false;
            }
            collisionCounter--;
        }
    }

    public override void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Deposit"))
        {
            collisionCounter--;
        }
        else
        {
            collisionCounter++;
            HealthComponent garbage;
            if (collider.gameObject.TryGetComponent<HealthComponent>(out garbage))
            {
                BoxCollider bCollider;
                if (gameObject.TryGetComponent<BoxCollider>(out bCollider))
                {
                    bCollider.isTrigger = true;
                }

            }
        }
    }

    #endregion

    #region IBuildable

    public override void LoadBuild(List<int> outputIDs, List<int> inputIDs, List<int> outputStacks, List<int> inputStacks)
    {
        base.LoadBuild(outputIDs, inputIDs, outputStacks, inputStacks);
        InitDrill();
    }

    public override void Build()
    {
        base.Build();
        InitDrill();
        Loid.Instance.UpdateTuto(PlayerAction.PlaceDrill);
    }

    public override void ResetCollisionCounter()
    {
        collisionCounter = 1;
    }

    #endregion

    #region IMachineInteractable

    public void OpenWindow()
    {
        MachineHUDManager.Instance.OpenDrillMenu(currentTransition, timer, currentTransition.Timer, isProducing);
        windowIsOpen = true;
        MachineHUDManager.Instance.OutputStackChange(itemsOutputStack);
        MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
        MachineHUDManager.Instance.onStackValueChange += AddStack;
    }

    public void CloseWindow()
    {
        MachineHUDManager.Instance.CloseDrillMenu();
        windowIsOpen = false;
        MachineHUDManager.Instance.onStackValueChange -= AddStack;
    }

    #endregion
    private void AddStack(int nbStackToAdd, bool isInInput, int index, SOItems itemType)
    {
        itemsOutputStack[index] += nbStackToAdd;
        itemsOutputType[index] = itemType;
    }

    public List<Attachement> GetInputAttachements()
    {
        return null;
    }

    public List<Attachement> GetOutputAttachements()
    {
        return output;
    }

    public void Select()
    {
        meshRenderer.gameObject.layer = 8;
    }
    public void Deselect()
    {
        if (isPlaced)
            meshRenderer.gameObject.layer = 0;
    }
    public void Interact()
    {
        if (IsPlaced)
        {
            if (ressourcesRayManager.LastSelected != null && this != ressourcesRayManager.LastSelected as Drill)
            {
                ressourcesRayManager.LastSelected.Deselect();
            }

            ressourcesRayManager.LastSelected = this;
            Select();

            if (!Player.Instance.GetOnDestructionMode())
            {
                ressourcesRayManager.InteractionMessage.SetMachineTextEnable(Type);
            }
            else
                ressourcesRayManager.InteractionMessage.SetDestructTextEnable(Type);
        }
    }
}
