using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceFruitTree : RessourceHarvestable, IDivisibleRessource
{
    private GameObject child;

    private float timerRespawn;
    private float initTimerRespawn;

    private bool isChildActive;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        child = this.transform.parent.GetChild(1).gameObject;
        initTimerRespawn = 3f;
        timerRespawn = initTimerRespawn;
        isChildActive = true;
    }

    private void Update()
    {
        ReactiveFruit();
    }

    public override bool Harvest()
    {
        if (PlayerInputManager.Instance.InteractIsPressed())
        {
            if (isChildActive)
            {
                if (base.Harvest())
                {
                    DeactiveChild();//mauvais nommage dEActive j'immagine plus DisableChild
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }

    private void ReactiveFruit()
    {
        if (!isChildActive)
        {
            timerRespawn -= Time.deltaTime;
            if (timerRespawn < 0)
            {
                child.SetActive(true);
                isChildActive = true;
                timerRespawn = initTimerRespawn;
            }
        }
    }

    public bool GetIsChildActive()
    {
        return isChildActive;
    }

    public void DeactiveChild()
    {
        Deselect();
        RessourcesRayManager.instance.NullifyLastSelected();

        child.SetActive(false);
        isChildActive = false;
    }
}
