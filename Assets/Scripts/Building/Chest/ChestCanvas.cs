using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestCanvas : MonoBehaviour
{
    #region Singleton

    private static ChestCanvas instance;
    public static ChestCanvas Instance { get => instance; }

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

    [SerializeField] private Transform chestSlotContainer;
    [SerializeField] private Transform playerSlotContainer;
    [SerializeField] private Transform hover;
    [SerializeField] private Transform divider;
    [SerializeField] private Transform drag;
    private int nbSlots = 24;

    public Transform ChestSlotContainer { get => chestSlotContainer; }
    public int NbSlots { get => nbSlots; }
    public Transform Divider { get => divider; }

    public delegate void OnItemValueChange(List<Item> items);
    public OnItemValueChange onItemValueChange;

    public void OpenCanvas()
    {
        PlayerInputManager.Instance.DisableCamera();
        PlayerInputManager.Instance.DisableMenuing();
        PlayerInputManager.Instance.DisableBuild();
        PlayerInputManager.Instance.DisableAction();
        PlayerInputManager.Instance.DisableMovement();
        PlayerInputManager.Instance.DisableCodex();
        PlayerInputManager.Instance.DisableBuildMenu();

        PlayerInputManager.Instance.EnableInventory();

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        Player.Instance.CreateInventory(playerSlotContainer, hover, divider, drag);


        Player.Instance.SetIsInMenu(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void CloseCanvas()
    {
        PlayerInputManager.Instance.EnableCamera();
        PlayerInputManager.Instance.EnableMenuing();
        PlayerInputManager.Instance.EnableBuild();
        PlayerInputManager.Instance.EnableAction();
        PlayerInputManager.Instance.EnableMovement();
        //PlayerInputManager.Instance.EnableCodex();
        PlayerInputManager.Instance.EnableBuildMenu();

        PlayerInputManager.Instance.DisableInventory();

        Player.Instance.DestroyInventory();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        Player.Instance.SetIsInMenu(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}