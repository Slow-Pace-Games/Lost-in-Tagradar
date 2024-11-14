using UnityEngine;
public class Assembler : WindowedMachine
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    private void Update()
    {
        if (IsPlaced || debug)
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

            GetItemsFromSplines();
            Produce();
            PutItemOnSpline();
            //PlaySound(); TODO
        }
    }
}