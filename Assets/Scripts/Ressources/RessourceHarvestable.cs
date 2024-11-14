using UnityEngine;

public class RessourceHarvestable : Ressource
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InitRessource();
    }

    public override bool Harvest()
    {
        if (PlayerInputManager.Instance.InteractIsPressed())
        {
            if (base.Harvest())
            {
                return true;
            }
            return false;
        }
        return false;
    }

    private void InitRessource()
    {
        // Init amount of ressource harvested
        if (sORessource.randomHarvestableCount)
        {
            harvestAmount = Random.Range(sORessource.minHarvestedAmount, sORessource.maxHarvestedAmount + 1);
        }
        else
        {
            harvestAmount = sORessource.minHarvestedAmount;
        }
    }
}