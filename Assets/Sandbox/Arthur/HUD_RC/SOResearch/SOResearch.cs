using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResearch", menuName = "Research")]
public class SOResearch : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private List<SOItems> resources;
    [SerializeField] private List<int> quantities;
    [SerializeField] public bool isUnlocked;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string reward;
    [SerializeField] private int duration;

    public List<SOItems> Resources { get => resources; }
    public List<int> Quantities { get => quantities; }
    public bool IsUnlocked { get => isUnlocked; }
    public Sprite Sprite { get => sprite; }
    public string Reward { get => reward; }
    public int Duration { get => duration; }
}
