using Cinemachine;
using UnityEngine;

public class PlayerInterract : MonoBehaviour
{
    public GameObject crossAir;
    public CinemachineVirtualCamera vc;
    private bool isInMenu = false;
    private IMachineInteractable currentMachine;
    private Camera fpsCam;

    public bool isHudOpen;

    public bool IsInMenu { get { return isInMenu; } }

    void Start()
    {
        PlayerInputManager.Instance.InteractMachineHudAction(OpenCloseBuildingMenu, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.CloseAction(CloseMenu, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.InteractAction(RecupItem, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.DestructAction(DestructBuilding, PlayerInputManager.ActionType.Add);
        fpsCam = Camera.main;
    }

    private void RecupItem()
    {
        if (Physics.Raycast(vc.transform.position, vc.transform.forward, out RaycastHit hit, 4f))
        {
            if (hit.transform.TryGetComponent<ItemInWorld>(out ItemInWorld item))
            {
                item.InteractItem();
            }
        }
    }

    public void OpenCloseBuildingMenu()
    {
        if (!isInMenu)
        {
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(/*transform.position*/rayOrigin, vc.transform.forward, out RaycastHit hit, 4f, 1 << LayerMask.NameToLayer("Default") ^ 1 << LayerMask.NameToLayer("Deposit") | 1 << LayerMask.NameToLayer("Selectable")))
            {
                if (hit.transform.TryGetComponent<IMachineInteractable>(out currentMachine))//open the hud
                {
                    if (!Player.Instance.GetOnDestructionMode())
                    {
                        MachineHUDManager.Instance.Clean();
                        currentMachine.OpenWindow();
                        OpenMenu();
                    }
                }
            }
        }
        else
        {
            CloseMenu();
        }
    }

    private void DestructBuilding()
    {
        if (Player.Instance.GetOnDestructionMode() && Physics.Raycast(vc.transform.position, vc.transform.forward, out RaycastHit hit, 4f))
        {
            if (hit.transform.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                destructible.Destruct();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(vc.transform.position, vc.transform.position + vc.transform.forward * 4f);
        Gizmos.color = Color.white;
    }

    private void OpenMenu()
    {
        crossAir.SetActive(false);
        Player.Instance.SetIsInMenu(true);

        isInMenu = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        PlayerInputManager.Instance.DisableMovement();
        PlayerInputManager.Instance.DisableBuild();
        PlayerInputManager.Instance.DisableMenuing();
        PlayerInputManager.Instance.DisableCamera();
        PlayerInputManager.Instance.DisableDebugKeys();
        PlayerInputManager.Instance.DisableAction();
        PlayerInputManager.Instance.DisableCodex();
        PlayerInputManager.Instance.DisableBuildMenu();

        PlayerInputManager.Instance.EnableInventory();
    }

    private void CloseFunctions()
    {
        crossAir.SetActive(true);
        Player.Instance.SetIsInMenu(false);

        currentMachine = null;
        isInMenu = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerInputManager.Instance.EnableMovement();
        PlayerInputManager.Instance.EnableBuild();
        PlayerInputManager.Instance.EnableMenuing();
        PlayerInputManager.Instance.EnableCamera();
        PlayerInputManager.Instance.EnableAction();
        PlayerInputManager.Instance.EnableDebugKeys();
        //PlayerInputManager.Instance.EnableCodex();
        PlayerInputManager.Instance.EnableBuildMenu();

        PlayerInputManager.Instance.DisableInventory();
    }

    public void CloseMenu()
    {
        if (isInMenu)
        {
            if (currentMachine != null)
            {
                currentMachine.CloseWindow();
            }

            CloseFunctions();
        }
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance == null)
        {
            return;
        }

        PlayerInputManager.Instance.InteractAction(OpenCloseBuildingMenu, PlayerInputManager.ActionType.Remove);
    }
}
