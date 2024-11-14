using UnityEngine;

public class UniqueBuilding : Building, ISelectable
{
    public bool hasAnInstance = false;
    [SerializeField] protected string type;
    public string Type { get { return type; } }

    protected RessourcesRayManager ressourcesRayManager = RessourcesRayManager.instance;

    protected void Update()
    {
        if (isPlaced)
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
        }
    }

    #region ISelectable
    public void Select()
    {
        meshRenderer.gameObject.layer = 8;
    }
    public void Deselect()
    {
        if (isPlaced)
            meshRenderer.gameObject.layer = 0;
    }
    public void Interact()
    {
        if (IsPlaced)
        {
            if (ressourcesRayManager.LastSelected != null && this != ressourcesRayManager.LastSelected as HUB)
            {
                ressourcesRayManager.LastSelected.Deselect();
            }

            ressourcesRayManager.LastSelected = this;
            Select();

            if (!Player.Instance.GetOnDestructionMode())
            {
                ressourcesRayManager.InteractionMessage.SetMachineTextEnable(Type);
            }
            else
                ressourcesRayManager.InteractionMessage.SetDestructTextEnable(Type);
        }
    }
    #endregion

    #region IBuildable
    public override bool CanBeBuilt(bool isSnapped)
    {
        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            if (Player.Instance.GetItemAmount(buildingData.costs[i].item) < buildingData.costs[i].value) //Check in inventory if the component are there in the adequate quantity
            {
                meshRenderer.material = materialPreview[1];
                return false;
            }
        }

        if (collisionCounter == 0 && !hasAnInstance && isSnapped)
        {
            meshRenderer.material = materialPreview[0];
            return true;
        }

        meshRenderer.material = materialPreview[1];
        return false;
    }
    #endregion
}
