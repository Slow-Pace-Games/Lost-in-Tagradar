using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class RecipeMachine : Machine, IMachineInteractable, ISelectable, IAttachement
{
    [Header("Recipe")]
    [SerializeField] protected SORecipe currentRecipe;
    protected float lastRecipeCost = 0;

    protected bool isProducing = false;

    [Header("Input/Output")]
    [SerializeField] protected List<Attachement> inputs;
    [SerializeField] protected List<Attachement> outputs;
    [SerializeField] protected List<int> orderedInputStack = new List<int>();

    [SerializeField] private string type;
    public string Type { get { return type; } }

    [Header("Emissive")]
    [SerializeField] protected bool hasSecondEmissive = false;

    private RessourcesRayManager ressourcesRayManager = RessourcesRayManager.instance;

    [SerializeField] Material matEmissive;

    public virtual void SetCurrentRecipe(SORecipe currentRecipe)
    {
        lastRecipeCost = currentRecipe.ElectricityCost;
        this.currentRecipe = currentRecipe;
        itemsOutputType[0] = currentRecipe.ItemOutput;
        timerMax = currentRecipe.MeltTime - buildingData.ranks[(int)tier].percentTimer;
        MachineHUDManager.Instance.MaxTimerChange(timerMax);
        orderedInputStack.Clear();
        foreach (SOInput input in currentRecipe.ItemsInput)
        {
            orderedInputStack.Add(0);
        }
    }

    public void OpenWindow()
    {
        //TODO envoyer les input/output type de la machine
        MachineHUDManager.Instance.OpenMachineMenu(buildingData.recipes, currentRecipe, timer, isAlimented);
        MachineHUDManager.Instance.onClickRecipe += SetCurrentRecipe;
        windowIsOpen = true;
        MachineHUDManager.Instance.OutputStackChange(itemsOutputStack);
        MachineHUDManager.Instance.InputStackChange(orderedInputStack);
        MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
        MachineHUDManager.Instance.onStackValueChange += AddStack;
    }

    public void CloseWindow()
    {
        MachineHUDManager.Instance.CloseMachineMenu();
        MachineHUDManager.Instance.onClickRecipe -= SetCurrentRecipe;
        windowIsOpen = false;
        MachineHUDManager.Instance.onStackValueChange -= AddStack;
    }

    private void ChangeEmissiveColor(float red, float green, float blue, bool hasSecondEmissive)
    {
        Color color = new Color(224, 224, 224);

        if (hasSecondEmissive && red > 0)
        {
            meshRenderer.materials[1].SetColor("_EmissiveColor", color * 0.0f);
        }
        else if (hasSecondEmissive && green > 0)
        {
            meshRenderer.materials[1].SetColor("_EmissiveColor", color *  0.05f);
        }

        var emissiveColor = meshRenderer.material.GetColor("_EmissiveColor");
        emissiveColor.r = red;
        emissiveColor.g = green;
        emissiveColor.b = blue;
        meshRenderer.material.SetColor("_EmissiveColor", emissiveColor);
    }

    private void SendDataToHUD()
    {
        if (WindowIsOpen)
        {
            MachineHUDManager.Instance.InputStackChange(itemsInputStack);
            MachineHUDManager.Instance.OutputStackChange(itemsOutputStack);
            MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
        }
    }

    private void UseElectricity()
    {
        float elecToChange = currentRecipe.ElectricityCost;
        elecToChange -= lastRecipeCost;

        if (isAlimented)
        {
            foreach (EnergyDistributor distributor in nearbyDistributors)
            {
                if (distributor.isGenerator)
                {
                    if ((distributor as Generator).usedElectricity + elecToChange <= (distributor as Generator).maxElectricity && (distributor as Generator).maxElectricity > 0)
                    {
                        return;
                    }
                }
                foreach (Generator generator in distributor.generators)
                {
                    if (generator.usedElectricity + elecToChange <= generator.maxElectricity && generator.maxElectricity > 0)
                    {
                        return;
                    }
                }
            }

            StopSound(audioSource1);
            isAlimented = false;
            MachineHUDManager.Instance.IsMachineAlimentedChange(isAlimented);
            isProducing = false;
            ChangeEmissiveColor(255, 0, 0, hasSecondEmissive);
            if (particle != null)
            {
                var main = particle.main;
                main.maxParticles = 0;
            }
            SendDataToHUD();
            lastRecipeCost = 0;
        }
        else
        {
            if (nearbyDistributors.Count == 0)
            {
                ChangeEmissiveColor(255, 0, 0, hasSecondEmissive);
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
            lastRecipeCost = currentRecipe.ElectricityCost;
            ChangeEmissiveColor(0, 255, 0, hasSecondEmissive);
            SendDataToHUD();
            return true;
        }
        return false;
    }

    protected virtual void Produce()
    {
        // Manage the alimentation of the machine
        if (currentRecipe != null)
        {
            UseElectricity();
        }

        // Manage the input resources of the machine
        if (!isProducing && currentRecipe != null && isAlimented)
        {
            int availableStacks = 0;
            for (int i = 0; i < itemsInputStack.Count; i++) //Browse the stack of the machine
            {
                foreach (SOInput item in currentRecipe.ItemsInput) //Browse the items in the recipe
                {
                    if (itemsInputType[i] == item.Item && itemsInputStack[i] >= item.ValueStack) //Check whether the type and the stack of the item correspond to the component in the recipe
                    {
                        availableStacks++;
                        break;
                    }
                }
            }

            if (availableStacks == itemsInputStack.Count)
            {
                isProducing = true;
                PlaySound(audioSource2, buildingData.sounds.GetRandomClip(MachineAudio.Work), false);
                if (particle != null)
                {
                    var main = particle.main;
                    main.maxParticles = 1000;
                }
                for (int i = 0; i < itemsInputStack.Count; i++) //Browse the input stacks of the machine
                {
                    foreach (SOInput item in currentRecipe.ItemsInput) //Browse the items in the recipe
                    {
                        if (itemsInputType[i] == item.Item && itemsInputStack[i] >= item.ValueStack) //Check whether the type and the stack of the item correspond to the component in the recipe
                        {
                            itemsInputStack[i] -= item.ValueStack;
                            break;
                        }
                    }
                }

                SendDataToHUD();
            }
        }

        // Manage the production and the output resources of the machine
        if (isProducing && isAlimented)
        {
            if (itemsOutputStack[0] < 100)
            {
                timer += Time.deltaTime;
            }

            if (timer >= timerMax)
            {
                PlaySound(audioSource2, buildingData.sounds.GetRandomClip(MachineAudio.Finish), false);
                itemsOutputStack[0] += currentRecipe.ValueStackOutput;
                isProducing = false;
                timer = 0.0f;
                SendDataToHUD();
            }
        }
    }

    protected virtual void GetItemsFromSplines()
    {
        if (currentRecipe != null) //Got a recipe
        {
            for (int i = 0; i < inputs.Count; ++i) //Browse the inputs of the machine
            {
                if (inputs[i].Convey != null && itemsInputStack[i] < 100) //There is a conveyor at the current input and the stack of the input is not saturated
                {
                    foreach (SOInput inputItem in currentRecipe.ItemsInput) //Browse the type of items in the current recipe of the machine
                    {
                        if (inputs[i].Convey.IsSameItemOnConvey(inputItem.Item)) //There is an item on the conveyor which is in the recipe
                        {
                            for (int j = 0; j < inputs.Count; ++j)
                            {
                                if (j == i && inputs[i].Convey.IsSameItemOnConvey(itemsInputType[j])) //There is an item on the conveyor and there was an item in the inputStack of the same type
                                {
                                    SOItems currentItem = inputs[i].Convey.GetItemOnConvey();
                                    if (currentItem != null)
                                    {
                                        itemsInputStack[i]++;
                                        SetOrderedList(itemsInputType[i], itemsInputStack[i]);
                                        SendDataToHUD();
                                    }
                                    break;
                                }
                                else if (inputs[i].Convey.IsSameItemOnConvey(itemsInputType[j])) //There is an item on the conveyor but the item is already supplied by another conveyor
                                {
                                    break;
                                }
                                else //There is an item on the conveyor and no other conveyor to supply it
                                {
                                    SOItems currentItem = inputs[i].Convey.GetItemOnConvey();
                                    if (currentItem != null)
                                    {
                                        itemsInputStack[i]++;
                                        itemsInputType[i] = currentItem;
                                        SetOrderedList(itemsInputType[i], itemsInputStack[i]);
                                        SendDataToHUD();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetOrderedList(SOItems item, int stack)
    {
        for (int j = 0; j < currentRecipe.ItemsInput.Count; ++j)
        {
            if (item == currentRecipe.ItemsInput[j].Item)
            {
                orderedInputStack[j] = stack;
                return;
            }
        }
    }

    protected virtual void PutItemOnSpline()
    {
        if (outputs[0].Convey != null && itemsOutputStack[0] > 0)
        {
            if (outputs[0].Convey.CanAddItemOnConvey())
            {
                outputs[0].Convey.AddItemOnConvey(currentRecipe.ItemOutput);
                itemsOutputStack[0]--;
            }
        }
    }

    private void AddStack(int nbStackToAdd, bool isInInput, int index, SOItems itemType)
    {
        if (isInInput)
        {
            for (int i = 0; i < itemsInputType.Count; ++i)
            {
                if (itemType == itemsInputType[i])
                {
                    itemsInputStack[i] += nbStackToAdd;
                    SetOrderedList(itemsInputType[i], itemsInputStack[i]);
                    return;
                }
            }

            for (int i = 0; i < itemsInputType.Count; ++i)
            {
                if (itemsInputType[i] == null)
                {
                    itemsInputStack[i] += nbStackToAdd;
                    itemsInputType[i] = itemType;
                    SetOrderedList(itemsInputType[i], itemsInputStack[i]);
                    return;
                }
            }
        }
        else
        {
            itemsOutputStack[index] += nbStackToAdd;
            itemsOutputType[index] = itemType;
        }

    }

    #region ISelectable

    public void Select()
    {
        meshRenderer.gameObject.layer = 8;
    }

    public void Deselect()
    {
        if (isPlaced)
        {
            meshRenderer.gameObject.layer = 0;
        }
    }

    public void Interact()
    {
        if (IsPlaced)
        {
            if (ressourcesRayManager.LastSelected != null && this != ressourcesRayManager.LastSelected as RecipeMachine)
            {
                ressourcesRayManager.LastSelected.Deselect();
            }

            ressourcesRayManager.LastSelected = gameObject.GetComponent<ISelectable>();
            Select();
            // Display message "Press "touch" to configure"
            if (!Player.Instance.GetOnDestructionMode())
            {
                ressourcesRayManager.InteractionMessage.SetMachineTextEnable(Type);
            }
            else
            {
                ressourcesRayManager.InteractionMessage.SetDestructTextEnable(Type);
            }
        }
    }

    #endregion

    public int GetIdCurrentRecipe()
    {
        for (int i = 0; i < buildingData.recipes.Count; i++)
        {
            if (buildingData.recipes[i] == currentRecipe)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetCurrentRecipe(int id)
    {
        if (id == -1)
        {
            currentRecipe = null;
            return;
        }
        currentRecipe = buildingData.recipes[id];
        SetCurrentRecipe(currentRecipe);
    }

    #region IAttachement

    public List<Attachement> GetInputAttachements()
    {
        return inputs;
    }

    public List<Attachement> GetOutputAttachements()
    {
        return outputs;
    }

    #endregion
}
