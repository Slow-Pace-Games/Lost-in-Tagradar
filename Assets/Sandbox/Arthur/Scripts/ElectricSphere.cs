using UnityEngine;

public class ElectricSphere : MonoBehaviour
{
    [Header("ParentComponent")]
    [SerializeField] EnergyDistributor energyDistributor;
    public void OnTriggerStay(Collider collider)
    {
        EnergyDistributor distributor;
        RecipeMachine machine;
        Drill drill;

        //Link to a energy source or pylon
        if (collider.gameObject.TryGetComponent(out distributor))
        {
            if (distributor != energyDistributor)
            {
                if (distributor.isGenerator && !energyDistributor.generators.Contains(distributor as Generator) && distributor.IsPlaced)
                {
                    energyDistributor.generators.Add(distributor as Generator);
                    ActivateBlueElecBall();
                }
                else if (!distributor.isGenerator && !energyDistributor.adjacentDistributors.Contains(distributor) && distributor.IsPlaced)
                {
                    energyDistributor.adjacentDistributors.Add(distributor);
                    ActivateBlueElecBall();
                }

                if (!distributor.isGenerator && distributor.IsPlaced)
                {
                    foreach (Generator generator in energyDistributor.generators)
                    {
                        if (!distributor.generators.Contains(generator))
                        {
                            distributor.generators.Add(generator);
                        }
                    }
                }
            }
        }
        //Link to a machine
        else if (collider.gameObject.TryGetComponent(out machine) && machine.IsPlaced)
        {
            if (energyDistributor.IsPlaced)
            {
                machine.AddNearbyDistributors = energyDistributor;
            }
            if (!energyDistributor.linkedMachine.Contains(machine))
            {
                energyDistributor.linkedMachine.Add(machine);
                ActivateBlueElecBall();
            }
        }
        //Link to a drill
        else if (collider.gameObject.TryGetComponent(out drill) && drill.IsPlaced)
        {
            if (energyDistributor.IsPlaced)
            {
                drill.AddNearbyDistributors = energyDistributor;
            }
            if (!energyDistributor.linkedMachine.Contains(drill))
            {
                energyDistributor.linkedMachine.Add(drill);
                ActivateBlueElecBall();
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        EnergyDistributor distributor;
        RecipeMachine machine;
        Drill drill;

        if (collider.gameObject.TryGetComponent(out distributor))
        {
            if (distributor.isGenerator)
            {
                energyDistributor.generators.Remove(distributor as Generator);
            }
            else
            {
                energyDistributor.adjacentDistributors.Remove(distributor);
            }
            energyDistributor.UpdateNode(energyDistributor);
        }
        else if (collider.gameObject.TryGetComponent(out machine))
        {
            energyDistributor.linkedMachine.Remove(machine);
        }
        else if (collider.gameObject.TryGetComponent(out drill))
        {
            energyDistributor.linkedMachine.Remove(drill);
        }


        if (!energyDistributor.IsPlaced)
        {
            if (energyDistributor.adjacentDistributors.Count == 0 && energyDistributor.generators.Count == 0 && energyDistributor.linkedMachine.Count == 0)
            {
                energyDistributor.electricBalls[0].SetActive(false);
                energyDistributor.electricBalls[1].SetActive(true);
            }
        }
    }

    private void ActivateBlueElecBall()
    {
        if (!energyDistributor.IsPlaced)
        {
            energyDistributor.electricBalls[1].SetActive(false);
            energyDistributor.electricBalls[0].SetActive(true);
        }
    }
}
