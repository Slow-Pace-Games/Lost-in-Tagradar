using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//position,rotation
//rank (enum -> int)
//current recipe -> int dans la list du scriptableObject database
//les connections pas besoin déjà fait dans les convey (voir code conveyorcontainer)

//coffre donc inventaire ptet faire un autre container juste pour eux

//Milestone 3 ptet les buffer interne

public class MachineContainer : SaveContainer
{
    //List convertie des enfants du gameobject (container)
    [SerializeField] private List<MachineSaveable> machines;
    [SerializeField] SODatabase soDataBase;

    protected override void Convert()
    {
        machines.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject currentMachine = transform.GetChild(i).gameObject;

            Building building = currentMachine.GetComponentInChildren<Building>();
            RecipeMachine recipeMachine = currentMachine.GetComponentInChildren<RecipeMachine>();

            MachineSaveable machineSaveable = new MachineSaveable();

            machineSaveable.name = currentMachine.name;

            if (recipeMachine != null)
            {
                machineSaveable.currentRecipe = recipeMachine.GetIdCurrentRecipe();
            }
            else
            {
                machineSaveable.currentRecipe = -1;
            }

            if (building is Machine)
            {
                machineSaveable.inputStack = (building as Machine).ItemsInputStack;
                machineSaveable.outputStack = (building as Machine).ItemsOutputStack;
                machineSaveable.inputID = (building as Machine).GetIdsItemInput();
                machineSaveable.outputID = (building as Machine).GetIdsItemOutput();
            }
            else if (building is Generator)
            {
                machineSaveable.inputStack = (building as Generator).ItemInputStack;
                machineSaveable.inputID = (building as Generator).GetIdsItemInput();
            }
            else if (building is Chest)
            {
                machineSaveable.inventory = new List<ItemSaveable>();

                for (int j = 0; j < (building as Chest).GetItemsInChest().Count; j++)
                {
                    machineSaveable.inventory.Add(default(ItemSaveable));
                    machineSaveable.inventory[j] = Serialize.ConvertSOItem((building as Chest).GetItemsInChest()[j]);
                }
            }

            dictionaryPrefabs.TryGetValue(machineSaveable.name, out machineSaveable.key);
            machineSaveable.transform = Serialize.ConvertSerialized(currentMachine.transform);

            machines.Add(machineSaveable);
        }

        SaveSystem.Instance.ClassContainer.machinesList = machines;
    }

    protected override void Load()
    {
        machines = SaveSystem.Instance.ClassContainer.machinesList;
        for (int i = 0; i < machines.Count; i++)
        {
            GameObject go = Instantiate(prefabs[machines[i].key], transform);
            RecipeMachine recipeMachine = go.GetComponentInChildren<RecipeMachine>();
            Machine machine = go.GetComponentInChildren<Machine>();
            Building building = go.GetComponentInChildren<Building>();


            if (recipeMachine != null)
            {
                recipeMachine.SetCurrentRecipe(machines[i].currentRecipe);
            }

            if (machine != null)
            {
                machine.IsPlaced = true;
            }

            go.name = machines[i].name;
            go.transform.position = Serialize.UnConvert(machines[i].transform, go.transform).position;
            go.transform.rotation = Serialize.UnConvert(machines[i].transform, go.transform).rotation;

            if (building != null)
            {
                if (building is Chest)
                {
                    //
                    Chest tempChest = (building as Chest);
                    (building as Chest).InitChest();
                    List<Item> tempInventory = new List<Item>();
                    for (int j = 0; j < tempChest.GetItemsInChest().Count; j++)
                    {
                        Item tempItem = new Item();

                        if (machines[i].inventory[j].id >= 0)
                        {
                            tempItem.itemType = soDataBase.AllItems.Where(item => item.id == machines[i].inventory[j].id).First(item => item);
                            tempItem.stacks = machines[i].inventory[j].number;
                        }
                        else
                        {
                            tempItem = null;
                        }


                        tempInventory.Add(tempItem);
                    }
                    (building as Chest).SetItemsInChest(tempInventory);
                    //
                }

                if (building is Drill)
                {
                    (building as Drill).LoadBuild(machines[i].outputID, machines[i].inputID, machines[i].outputStack, machines[i].inputStack);
                }
                else if (building is Generator)
                {
                    (building as Generator).LoadBuild(machines[i].outputID, machines[i].inputID, machines[i].outputStack, machines[i].inputStack);
                }
                else
                {
                    building.LoadBuild(machines[i].outputID, machines[i].inputID, machines[i].outputStack, machines[i].inputStack);
                }
            }
        }
        ElecBallsManager.Instance.HideElecBalls();
    }
}

[System.Serializable]
public class MachineSaveable
{
    public int key;
    public string name;
    public int currentRecipe;
    public List<int> inputStack = new List<int>();
    public List<int> outputStack = new List<int>();
    public List<int> inputID = new List<int>();
    public List<int> outputID = new List<int>();
    public List<ItemSaveable> inventory;
    public TransformSerialized transform;
}