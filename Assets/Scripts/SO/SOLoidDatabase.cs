using UnityEngine;

[CreateAssetMenu(menuName = "Loid/Loid Database", fileName = "LoidDatabase")]
public class SOLoidDatabase : ScriptableObject
{
    [SerializeField] private SOLoidTips[] tips;
    [SerializeField] private SOLoidTuto[] tuto;

    public SOLoidTips[] Tips { get => tips; }
    public SOLoidTuto[] Tuto { get => tuto; }
}