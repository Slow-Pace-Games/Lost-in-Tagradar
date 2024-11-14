using UnityEngine;

[CreateAssetMenu(fileName = "Ressource", menuName = "Sandbox/David/Ressource", order = 0)]
public class SORessource : ScriptableObject
{
    public enum Type
    {
        ///// The order must be the same as ScanableResources enum /////

        // Exploitable ressources
        Basilite,
        Agulite,
        Scenarium,
        Boznite,
        Dacium,

        // Ressources count
        Count
    }

    public Type type;
    public SOItems item;

    [Tooltip("Is the count of earn ressource random ?")]
    public bool randomHarvestableCount;

    [Tooltip("! If non random, min value is taken !\nThe min amount of ressources collected in one time")]
    public int minHarvestedAmount;

    [Tooltip("! If non random, min value is taken !\nThe max amount of ressources collected in one time")]
    public int maxHarvestedAmount;

    [Tooltip("Can the ressource be detroy ?")]
    public bool isDestroyable;

    [Tooltip("Can the ressource respawn ?")]
    public bool isRespawnable;

    [Tooltip("! Only if respawnable !\nTime before ressource respawning")]
    public float timerRespawn;

    [Tooltip("Is the ressource need several harvest before destroying")]
    public bool isDurable;

    [Tooltip("! Only if durable !\nThe min amount of times the ressource can be collected before destruction")]
    public int minDurability;

    [Tooltip("! Only if durable !\nThe max amount of times the ressource can be collected before destruction")]
    public int maxDurability;
}