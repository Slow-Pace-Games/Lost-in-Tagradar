using UnityEngine;

public class Spaceship : MonoBehaviour, IMachineInteractable, ISelectable
{
    [Header("Render")]
    [SerializeField] private GameObject spaceship;

    [Header("Canvas")]
    [SerializeField] private SpaceshipCanvas spaceshipCanvas;

    [SerializeField] private string type;
    private bool isFullyRepaired;
    public bool IsFullyRepaired { get { return isFullyRepaired; } }
    public string Type { get { return type; } }

    private RessourcesRayManager ressourcesRayManager;

    private void Start()
    {
        ressourcesRayManager = RessourcesRayManager.instance;
    }

    #region IMachineInteractable
    public void OpenWindow()
    {
        Loid.Instance.UpdateTuto(PlayerAction.OpenShip);
        SpaceshipCanvas.Instance.onAllPartsRepaired += IsAllPartsRepaired;
        spaceshipCanvas.OpenSpaceshipCanvas();
    }

    public void CloseWindow()
    {
        SpaceshipCanvas.Instance.onAllPartsRepaired -= IsAllPartsRepaired;
        spaceshipCanvas.CloseSpaceshipCanvas();
    }
    #endregion

    #region ISelectable
    public void Select()
    {
        spaceship.layer = 8;
    }
    public void Deselect()
    {
        spaceship.layer = 0;
    }
    public void Interact()
    {
        if (ressourcesRayManager.LastSelected != null && this != ressourcesRayManager.LastSelected as Spaceship)
        {
            ressourcesRayManager.LastSelected.Deselect();
        }

        ressourcesRayManager.LastSelected = this.GetComponent<ISelectable>();
        Select();

        ressourcesRayManager.InteractionMessage.SetMachineTextEnable(Type);
    }
    #endregion

    private void IsAllPartsRepaired()
    {
        isFullyRepaired = true;
    }

}
