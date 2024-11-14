using System.Collections.Generic;
using UnityEngine;
public class Divider : Machine, IAttachement
{
    [Header("Inputs/Output")]
    [SerializeField] List<Attachement> outputs;
    [SerializeField] List<Attachement> input;
    public int outputID = 0;
    private int nbOutput = 3;

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
            Divide();
            PutItemOnSpline();
            //PlaySound(); TODO
        }
    }

    private void GetItemsFromSplines()
    {
        if (input[0].Convey != null && itemsInputStack[0] < 100 && input[0].Convey.CanGetItemOnConvey())
        {
            if (itemsInputType[0] != null && input[0].Convey.IsSameItemOnConvey(itemsInputType[0]))
            {
                SOItems currentItem = input[0].Convey.GetItemOnConvey();
                itemsInputStack[0]++;
            }
            else if (itemsInputType[0] == null)
            {
                SOItems currentItem = input[0].Convey.GetItemOnConvey();
                itemsInputStack[0]++;
                itemsInputType[0] = currentItem;
                itemsOutputType[0] = itemsInputType[0];
            }
        }
    }

    private void PutItemOnSpline()
    {
        if (outputs[0].Convey != null && itemsOutputStack[0] > 0)
        {
            if (outputs[0].Convey.CanAddItemOnConvey())
            {
                outputs[0].Convey.AddItemOnConvey(itemsOutputType[0]);
                itemsOutputStack[0]--;
            }
        }

        if (outputs[1].Convey != null && itemsOutputStack[1] > 0)
        {
            if (outputs[1].Convey.CanAddItemOnConvey())
            {
                outputs[1].Convey.AddItemOnConvey(itemsOutputType[0]);
                itemsOutputStack[1]--;
            }
        }

        if (outputs[2].Convey != null && itemsOutputStack[2] > 0)
        {
            if (outputs[2].Convey.CanAddItemOnConvey())
            {
                outputs[2].Convey.AddItemOnConvey(itemsOutputType[0]);
                itemsOutputStack[2]--;
            }
        }
    }

    private void Divide()
    {
        if (itemsInputStack[0] > 0)
        {
            itemsInputStack[0]--;
            itemsOutputStack[outputID]++;
            outputID++;
            outputID %= nbOutput;
        }
    }

    public List<Attachement> GetInputAttachements()
    {
        return input;
    }

    public List<Attachement> GetOutputAttachements()
    {
        return outputs;
    }
}