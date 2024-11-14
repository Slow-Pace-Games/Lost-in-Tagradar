using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe", order = 2)]
public class SORecipe : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string nameRecipe;

    public string NameRecipe { get => nameRecipe; set => nameRecipe = value; }

    [Header("Output")]
    [SerializeField] private SOItems itemOutput;
    [SerializeField] private int valueStackOutput;

    public SOItems ItemOutput { get => itemOutput; set => itemOutput = value; }
    public int ValueStackOutput { get => valueStackOutput; set => valueStackOutput = value; }

    [Header("Input")]
    [SerializeField] private List<SOInput> itemsInput = new List<SOInput>();

    public List<SOInput> ItemsInput { get => itemsInput; set => itemsInput = value; }

    [Header("Info")]
    [SerializeField] float meltTime;
    [SerializeField] float electricityCost;
    [SerializeField, TextArea(10, 10)] string description;
    public bool isUnlock;
    public float MeltTime { get => meltTime; set => meltTime = value; }
    public float ElectricityCost { get => electricityCost; set => electricityCost = value; }
    public string Description { get => description; set => description = value; }

    public MachineType machineRef = MachineType.None;
}

[System.Serializable]
public class SOInput
{
    [Header("Input")]
    [SerializeField] private SOItems inputItem;
    [SerializeField] private int valueStack;

    public SOItems Item { get => inputItem; set => inputItem = value; }
    public int ValueStack { get => valueStack; set => valueStack = value; }
}