using UnityEngine;

public class Furnace : RecipeMachine
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
            Produce();
            PutItemOnSpline();
        }
    }

    protected override void PutItemOnSpline()
    {
        if (outputs[0].Convey != null && itemsOutputStack[0] > 0)
        {
            if (outputs[0].Convey.CanAddItemOnConvey())
            {
                outputs[0].Convey.AddItemOnConvey(currentRecipe.ItemOutput);
                itemsOutputStack[0]--;
                MachineHUDManager.Instance.OutputStackChange(itemsOutputStack);
                //PlaySound(); TODO
            }
        }
    }

    public override void Build()
    {
        base.Build();
        Loid.Instance.UpdateTuto(PlayerAction.PlaceFurnace);
    }
}