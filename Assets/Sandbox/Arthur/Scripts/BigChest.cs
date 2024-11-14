using System.Collections.Generic;
using UnityEngine;

public class BigChest : Chest, IColliderIO, IAttachement
{
    [Header("Input/Output")]
    [SerializeField] private Collider[] inputOutput;
    [SerializeField] private List<Attachement> input;
    [SerializeField] private List<Attachement> output;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    private void Start()
    {
        InitChest();
    }

    private void Update()
    {
        if (IsPlaced || debug)
        {
            if (instantiatedEffect != null)
            {
                effectTimer += TimeScale.deltaTime;
                if (effectTimer >= effectMaxTimer)
                {
                    Destroy(instantiatedEffect.gameObject);
                    ActivateMesh(true);
                }
            }

            GetItemFromSpline();
            PutItemOnSpline();
        }
    }

    private void GetItemFromSpline()
    {
        if (input[0].Convey != null && input[0].Convey.CanGetItemOnConvey())
        {
            for (int i = 0; i < inventory.Count; ++i)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = new Item();
                    inventory[i].itemType = input[0].Convey.GetItemOnConvey();
                    inventory[i].stacks++;
                    if (isWindowOpen)
                    {
                        chestSlotContainer.GetChild(i).GetComponent<ChestSlot>().UpdateSlot(inventory[i]);
                    }
                    break;
                }
                else if (input[0].Convey.IsSameItemOnConvey(inventory[i].itemType) && inventory[i].stacks < 100)
                {
                    input[0].Convey.GetItemOnConvey();
                    inventory[i].stacks++;
                    if (isWindowOpen)
                    {
                        chestSlotContainer.GetChild(i).GetComponent<ChestSlot>().UpdateSlot(inventory[i]);
                    }
                    break;
                }
            }
        }
    }

    private void PutItemOnSpline()
    {
        int id = -1;
        if (output[0].Convey != null)
        {
            if (output[0].Convey.CanAddItemOnConvey())
            {
                for (int i = inventory.Count - 1; i >= 0; --i)
                {
                    if (inventory[i] != null && inventory[i].stacks > 0)
                    {
                        id = i;
                        break;
                    }

                }
                if (id != -1)
                {
                    output[0].Convey.AddItemOnConvey(inventory[id].itemType);
                    inventory[id].stacks--;
                    if (isWindowOpen)
                    {
                        chestSlotContainer.GetChild(id).GetComponent<ChestSlot>().UpdateSlot(inventory[id]);
                    }
                }
            }
        }
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
            collider.enabled = false;
        }
    }

    #endregion

    public List<Attachement> GetInputAttachements()
    {
        return input;
    }

    public List<Attachement> GetOutputAttachements()
    {
        return output;
    }
}