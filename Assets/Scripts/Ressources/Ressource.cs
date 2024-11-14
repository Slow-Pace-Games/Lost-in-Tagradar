using UnityEngine;
using UnityEngine.UIElements;

public class Ressource : MonoBehaviour, ISelectable, Iharvestable
{
    public SORessource sORessource;

    private int durability;
    protected int harvestAmount;

    private RessourcesRayManager ressourcesRayManager;

    public string Type { get { return sORessource.type.ToString(); } }
    public SOItems Item { get { return sORessource.item; } }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ressourcesRayManager = RessourcesRayManager.instance;
        Init();
    }

    public void Select()
    {
        transform.GetComponentInChildren<MeshRenderer>().gameObject.layer = 8; // Change layer to "Selectable"
    }

    public void Deselect()
    {
        PlayerUi.Instance.UpdateFillAmountMinage(0);
        transform.GetComponentInChildren<MeshRenderer>().gameObject.layer = 9; // Change layer to "Deposit"
    }

    public void Interact()
    {
        if (ressourcesRayManager.LastSelected != null && gameObject != ressourcesRayManager.LastSelected as Ressource)
        {
            if (ressourcesRayManager.IsObjectSelected)
            {

                ressourcesRayManager.LastSelected.Deselect();
            }
        }

        ressourcesRayManager.LastSelected = gameObject.GetComponent<ISelectable>();
        ressourcesRayManager.DivisibleRessource = gameObject.GetComponent<IDivisibleRessource>();
        Select();

        if (ressourcesRayManager.DivisibleRessource != null)
        {
            if (ressourcesRayManager.DivisibleRessource.GetIsChildActive())
            {
                // Display message "Press "touch" to pick up ..."
                ressourcesRayManager.InteractionMessage.SetRessourceTextEnable(Type);
            }
            else
            {
                ressourcesRayManager.OnDeselectInvoke();
            }
        }
        else
        {
            // Display message "Press "touch" to pick up ..."
            ressourcesRayManager.InteractionMessage.SetRessourceTextEnable(Type);
        }

        if (Player.Instance.CanAddItem(Item))
        {
            if (Harvest())
            {
                // Display amount and type of harvested ressource
                int itemAmount = Player.Instance.GetItemAmount(Item);
                ressourcesRayManager.RessourcesInfos.SetText("+ " + harvestAmount + " " + Type + " (" + itemAmount + ")");

                // Display message "nbr resources harvested"
                ressourcesRayManager.RessourcesInfos.EnableMessage();
            }
        }
        else
        {
            // Display message "Not enough place to pick up ..."
            ressourcesRayManager.InteractionMessage.SetUnpickableRessourceTextEnable(Type);
        }
    }

    public virtual bool Harvest()
    {
        AddRessource();
        return true;
    }

    private void AddRessource()
    {
        Player.Instance.AddItem(sORessource.item, harvestAmount);
        if (!sORessource.item.IsDiscover)
        {
            sORessource.item.IsDiscover = true;
            Player.Instance.AddScannableResource((int)sORessource.type);
        }
        if (sORessource.isDestroyable)
        {
            durability--;
            if (durability <= 0)
            {
                DestroyRessource();
            }
        }
        Player.Instance.PlayRandomSoundInPickUp();
    }

    protected void Init()
    {
        // Init durability
        if (sORessource.isDurable)
        {
            durability = Random.Range(sORessource.minDurability, sORessource.maxDurability);
        }
        else
        {
            durability = 1;
        }
    }

    private void DestroyRessource()
    {
        Deselect();
        ressourcesRayManager.NullifyLastSelected();

        RessourceData newData = new RessourceData();

        if (sORessource.isRespawnable)
        {
            newData.type = sORessource.type;
            newData.position = this.transform.position;
            newData.rotation = this.transform.rotation;
            newData.timer = sORessource.timerRespawn;
            RessourcesManager.instance.listData.Add(newData);
        }

        //for (int i = 0; i < gameObject.transform.childCount; i++)
        //{
        //    Destroy(gameObject.transform.GetChild(i).gameObject);
        //}
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
