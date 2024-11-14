using UnityEngine;

[CreateAssetMenu(fileName = "NewTransition", menuName = "Transition")]
public class SOTransition : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private SOItems resource;
    [SerializeField] private int quantity;
    [SerializeField] private float timer;
    [SerializeField] private float electricityCost;

    public SOItems Resource { get => resource; }
    public int Quantity { get => quantity; }
    public float Timer { get => timer; }
    public float ElectricityCost { get => electricityCost; }
}
