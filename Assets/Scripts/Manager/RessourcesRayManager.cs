using System;
using UnityEngine;

//je pense que ce code pourrait être rapatrier sur le player directement comment ça on pourrait mieux gérer le coté caméra et sa aurait aussi du sens

public class RessourcesRayManager : MonoBehaviour
{
    public static RessourcesRayManager instance;

    // UnDisplay Interaction message
    public event Action OnDeselectObject;

    private ISelectable lastSelected;
    private IDivisibleRessource divisibleRessource;
    private bool isObjectSelected;

    ////Raycast from screen center
    private Camera fpsCam;
    private Vector3 rayOrigin;

    [Tooltip("Max distance of the raycast")]
    [SerializeField] float rayRange = 4f;
    ////

    //// HUD
    [SerializeField] RessourcesInfos ressourcesInfos;
    [SerializeField] InteractionMessage interactionMessage;
    ////
    public ISelectable LastSelected { get => lastSelected; set => lastSelected = value; }
    public IDivisibleRessource DivisibleRessource { get => divisibleRessource; set => divisibleRessource = value; }
    public bool IsObjectSelected { get => isObjectSelected; set => isObjectSelected = value; }
    public RessourcesInfos RessourcesInfos { get => ressourcesInfos; set => ressourcesInfos = value; }
    public InteractionMessage InteractionMessage { get => interactionMessage; set => interactionMessage = value; }

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

        OnDeselectObject += Deselect;
    }

    // Start is called before the first frame update
    private void Start()
    {
        lastSelected = null;
        isObjectSelected = false;

        //Raycast from screen center
        fpsCam = Camera.main;
    }

    private void Update()
    {
        if (PlayerInputManager.Instance.IsActionEnable())
        {
            RaycastHit hit;

            //// Raycast with mouse position
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray, out hit))
            ////

            //// Raycast from screen center
            rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0f));
            ////
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, rayRange, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Deposit") | 1 << LayerMask.NameToLayer("Selectable") | 1 << LayerMask.NameToLayer("Items")))
            {
                ISelectable selectable = hit.transform.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    selectable.Interact();
                    if (!isObjectSelected)
                    {
                        isObjectSelected = true;
                    }
                }
                else
                {
                    OnDeselectObject?.Invoke();
                }
            }
            else
            {
                OnDeselectObject?.Invoke();
            }
        }
        else
        {
            OnDeselectObject?.Invoke();
        }
    }

    private void Deselect()
    {
        if (isObjectSelected)
        {
            isObjectSelected = false;

            if (lastSelected != null)
            {
                lastSelected.Deselect();
            }
            divisibleRessource = null;
        }
    }

    public void OnDeselectInvoke()
    {
        OnDeselectObject?.Invoke();
    }

    public void NullifyLastSelected()
    {
        lastSelected = null;
    }
}