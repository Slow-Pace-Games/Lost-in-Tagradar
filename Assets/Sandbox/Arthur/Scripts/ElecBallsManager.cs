using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecBallsManager : MonoBehaviour
{
    [SerializeField] private List<EnergyDistributor> distributors = new List<EnergyDistributor>();

    #region Singleton
    private static ElecBallsManager instance;
    public static ElecBallsManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void AddDistributor(EnergyDistributor distributor)
    {
        if (!distributors.Contains(distributor))
        {
            distributors.Add(distributor);
        }
    }

    public void DeleteDistributor(EnergyDistributor distributor)
    {
        distributors.Remove(distributor);
    }

    public void ShowElecBalls()
    {
        foreach (EnergyDistributor distributor in distributors)
        {
            if (distributor.adjacentDistributors.Count == 0 && distributor.generators.Count == 0 && distributor.linkedMachine.Count == 0)
            {
                distributor.electricBalls[0].SetActive(false);
                distributor.electricBalls[1].SetActive(true);
            }
            else
            {
                distributor.electricBalls[1].SetActive(false);
                distributor.electricBalls[0].SetActive(true);
            }
        }
    }

    public void HideElecBalls()
    {
        foreach (EnergyDistributor distributor in distributors)
        {
            distributor.electricBalls[0].SetActive(false);
            distributor.electricBalls[1].SetActive(false);
        }
    }
}
