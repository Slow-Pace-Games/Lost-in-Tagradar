using UnityEngine;
public class Desassembler : RecipeMachine
{
    [Header("Debug")]
    [SerializeField] private bool debug;
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

            GetItemsFromSplines();
            Deassemble();
            PutItemOnSpline();
            //PlaySound(); TODO
        }
    }

    protected override void GetItemsFromSplines()
    {
        if (currentRecipe != null && inputs[0] != null && itemsInputStack[0] < 100)
        {
            SOItems currentItem = inputs[0].Convey.GetItemOnConvey();
            if (currentItem != null && itemsInputType[0] == null)
            {
                if (currentItem == currentRecipe.ItemOutput)
                {
                    itemsInputStack[0]++;
                    itemsInputType[0] = currentItem;
                    if (WindowIsOpen)
                    {
                        MachineHUDManager.Instance.InputStackChange(itemsInputStack);
                    }
                }
            }
            else if (itemsInputType[0] != null && currentItem == itemsInputType[0])
            {
                itemsInputStack[0]++;
                if (WindowIsOpen)
                {
                    MachineHUDManager.Instance.InputStackChange(itemsInputStack);
                }
            }
        }
    }

    protected override void PutItemOnSpline()
    {
        if (outputs[0].Convey != null && itemsOutputStack[0] > 0)
        {
            if (outputs[0].Convey.CanAddItemOnConvey())
            {
                outputs[0].Convey.AddItemOnConvey(currentRecipe.ItemsInput[0].Item);
                itemsOutputStack[0]--;
            }
        }

        if (outputs[1].Convey != null && itemsOutputStack[1] > 0)
        {
            if (outputs[1].Convey.CanAddItemOnConvey())
            {
                outputs[1].Convey.AddItemOnConvey(currentRecipe.ItemsInput[1].Item);
                itemsOutputStack[1]--;
            }
        }
    }

    private void Deassemble()
    {
        if (itemsInputStack[0] > 0 && !isProducing)
        {
            isProducing = true;
            itemsInputStack[0] -= currentRecipe.ItemsInput[0].ValueStack;
            if (WindowIsOpen)
            {
                MachineHUDManager.Instance.InputStackChange(itemsInputStack);
                MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
            }
        }

        if (isProducing)
        {
            if (itemsOutputStack[0] < 100 && itemsOutputStack[1] < 100)
            {
                timer += Time.deltaTime;
            }

            if (timer >= timerMax)
            {
                itemsOutputStack[0]++;
                itemsOutputStack[1]++;
                isProducing = false;
                timer = 0.0f;
                if (WindowIsOpen)
                {
                    MachineHUDManager.Instance.OutputStackChange(itemsOutputStack);
                    MachineHUDManager.Instance.IsMachineProducingChange(isProducing);
                }
            }
        }
    }
}
