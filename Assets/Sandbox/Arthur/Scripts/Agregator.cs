using System.Collections.Generic;
using UnityEngine;
public class Agregator : Machine, IAttachement
{
    [Header("Inputs/Output")]
    [SerializeField] List<Attachement> inputs;
    [SerializeField] List<Attachement> output;

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
            Agregate();
            PutItemOnSpline();
            //PlaySound(); TODO
        }
    }

    private void GetItemsFromSplines()
    {
        for (int i = 0; i < inputs.Count; ++i)
        {
            if (inputs[i].Convey != null && itemsInputStack[i] < 100 && inputs[i].Convey.CanGetItemOnConvey())
            {
                SOItems currentItem = inputs[i].Convey.GetItemOnConvey();
                if (itemsOutputType[0] == null)
                {
                    itemsOutputType[0] = currentItem;
                }

                if (itemsInputType[i] == null)
                {
                    itemsInputStack[i]++;
                    itemsInputType[i] = currentItem;
                }
                else if (itemsInputType[i] != null && currentItem == itemsInputType[i])
                {
                    itemsInputStack[i]++;
                }
            }
        }
    }

    private void PutItemOnSpline()
    {
        if (itemsOutputType[0] != null && output[0].Convey != null && itemsOutputStack[0] > 0)
        {
            if (output[0].Convey.CanAddItemOnConvey())
            {
                output[0].Convey.AddItemOnConvey(itemsOutputType[0]);
                itemsOutputStack[0]--;
            }
        }
    }

    private void Agregate()
    {
        for (int i = 0; i < inputs.Count; ++i)
        {
            if (itemsInputType[i] != null && itemsInputStack[i] > 0 && itemsOutputStack[0] < 100)
            {
                itemsInputStack[i]--;
                itemsOutputStack[0]++;
            }
        }
    }

    public List<Attachement> GetInputAttachements()
    {
        return inputs;
    }

    public List<Attachement> GetOutputAttachements()
    {
        return output;
    }
}