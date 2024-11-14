using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WindowedMachine : RecipeMachine
{
    public override bool CanBeBuilt(bool isSnapped)
    {
        Material[] mrMat = meshRenderer.materials;

        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            if (Player.Instance.GetItemAmount(buildingData.costs[i].item) < buildingData.costs[i].value) //Check in inventory if the component are there in the adequate quantity
            {
                for (int j = 0; j < mrMat.Length; ++j)
                {
                    mrMat[j] = materialPreview[1];
                }
                meshRenderer.materials = mrMat;
                return false;
            }
        }

        if (collisionCounter == 0 && isSnapped)
        {
            for (int j = 0; j < mrMat.Length; ++j)
            {
                mrMat[j] = materialPreview[0];
            }
            meshRenderer.materials = mrMat;
            return true;
        }

        for (int j = 0; j < mrMat.Length; ++j)
        {
            mrMat[j] = materialPreview[1];
        }
        meshRenderer.materials = mrMat;
        return false;
    }

    public override void Build()
    {
        for (int i = 0; i < inputOutput.Length; i++)
        {
            inputOutput[i].enabled = true;
        }
        gameObject.GetComponent<NavMeshObstacle>().enabled = true;

        Material[] mrMat = meshRenderer.materials;
        if (mrMat.Length <= 2)
        {
            mrMat[0] = materialPreview[2];
            mrMat[1] = materialPreview[3];
        }
        else
        {
            mrMat[0] = materialPreview[4];
            mrMat[1] = materialPreview[2];
            mrMat[2] = materialPreview[3];
        }
        
        meshRenderer.materials = mrMat;

        IsPlaced = true;

        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            Player.Instance.RemoveItem(buildingData.costs[i].item, buildingData.costs[i].value);
        }

        instantiatedEffect = Instantiate(effect, transform.position, transform.rotation);
        instantiatedEffect.Play();

        SetArrowActive(false);
    }
}
