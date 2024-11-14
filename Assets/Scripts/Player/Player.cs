using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Singleton
    private static Player instance;
    public static Player Instance { get => instance; }
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

        playerMovements = GetComponent<PlayerMovements>();
        playerInventory = GetComponent<PlayerInventory>();
        cameraMovements = GetComponent<CameraMovements>();
        playerInterract = GetComponent<PlayerInterract>();
        playerBuildance = GetComponent<PlayerBuildance>();
        healthComponent = GetComponent<HealthComponent>();
        hotBarSelection = GetComponent<HotBarSelection>();
        playerScanner = GetComponent<PlayerScanner>();
        playerSound = GetComponent<PlayerSound>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }
    #endregion

    private PlayerMovements playerMovements;
    private PlayerInventory playerInventory;
    private CameraMovements cameraMovements;
    private PlayerInterract playerInterract;
    private PlayerBuildance playerBuildance;
    private HealthComponent healthComponent;
    private HotBarSelection hotBarSelection;
    private PlayerScanner playerScanner;
    private PlayerSound playerSound;
    private PlayerAnimation playerAnimation;
    [SerializeField] private HealthUI healthUI;

    private Vector3 respawnPos;
    private bool IsPauseOpen = false;
    private bool isPauseOpen
    {
        get => IsPauseOpen;
        set
        {
            IsPauseOpen = value;
        }
    }

    public HotBarSelection HotBarSelection { get => hotBarSelection; }
    public PlayerBuildance PlayerBuildance { get => playerBuildance; }

    #region Init/Update
    private void Start()
    {
        respawnPos = transform.position;
        isPauseOpen = false;
        SetSensX(SettingsManager.Instance.settingsToSave.playerSetSaveable.mouseSensitivityX);
        SetSensY(SettingsManager.Instance.settingsToSave.playerSetSaveable.mouseSensitivityY);

        PlayerInputManager.Instance.OpenPauseMenuAction(OpenPause, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.ClosePauseMenuAction(ClosePause, PlayerInputManager.ActionType.Add);
    }

    private void OnDisable()
    {
        isPauseOpen = false;

        if (PlayerInputManager.Instance == null)
        {
            return;
        }

        PlayerInputManager.Instance.OpenPauseMenuAction(OpenPause, PlayerInputManager.ActionType.Remove);
        PlayerInputManager.Instance.ClosePauseMenuAction(ClosePause, PlayerInputManager.ActionType.Remove);
    }

    private void Update()
    {
        if (hotBarSelection.isBuilding && !GetIsInMenu())
        {
            playerBuildance.UpdateBuild();
        }
        hotBarSelection.UpdateHotBar();
    }
    #endregion

    #region Misc
    private void OpenPause()
    {
        PlayerInputManager.Instance.DisableCamera();
        PlayerInputManager.Instance.DisableDebugKeys();
        PlayerInputManager.Instance.DisableHudMachine();
        PlayerInputManager.Instance.DisableBuild();
        PlayerInputManager.Instance.DisableInventory();
        PlayerInputManager.Instance.DisableAction();
        PlayerInputManager.Instance.DisableMovement();

        PlayerInputManager.Instance.EnableMenuing();

        SetIsInMenu(true);

        PauseManager.Instance.TogglePause(true);

        isPauseOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
    private void ClosePause()
    {
        PlayerInputManager.Instance.EnableCamera();
        PlayerInputManager.Instance.EnableDebugKeys();
        PlayerInputManager.Instance.EnableHudMachine();
        PlayerInputManager.Instance.EnableBuild();
        PlayerInputManager.Instance.EnableInventory();
        PlayerInputManager.Instance.EnableAction();
        PlayerInputManager.Instance.EnableMovement();

        PlayerInputManager.Instance.DisableMenuing();

        SetIsInMenu(false);

        PauseManager.Instance.TogglePause(false);

        isPauseOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Pause() => ClosePause();

    public void Respawn()
    {
        transform.position = respawnPos;

        healthUI.Stop();
        healthComponent.currentHealthPoint = healthComponent.maxHealthPoint;

        healthUI.ResetHealthBar();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    }
    #endregion

    #region Inventory
    public List<Item> GetInventory()
    {
        return playerInventory.inventory;
    }

    public void SetInventory(List<Item> _inventory)
    {
        playerInventory.inventory = _inventory;
    }

    public void AddInventorySlot()
    {
        playerInventory.AddInventorySlot();
    }

    public void CreateInventory(Transform _parent, Transform _hover, Transform _divider, Transform _drag)
    {
        playerInventory.CreateInventory(_parent, _hover, _divider, _drag);
    }

    public void DestroyInventory()
    {
        playerInventory.DestroyInventory();
    }

    public void AddItem(SOItems _item, int _stacks)
    {
        playerInventory.AddItem(_item, _stacks);
    }

    public void RemoveItem(SOItems _item, int _stacks)
    {
        playerInventory.RemoveItem(_item, _stacks);
    }

    public bool CanAddItem()
    {
        return playerInventory.CanAddItem();
    }

    public bool CanAddItem(SOItems _item)
    {
        return playerInventory.CanAddItem(_item);
    }

    public int GetItemAmount(SOItems _item)
    {
        return playerInventory.GetItemAmount(_item);
    }

    public int GetStackAvailable(SOItems _item)
    {
        return playerInventory.GetStackAvailable(_item);
    }

    public void UpdateInventory(int _index, Item _item)
    {
        playerInventory.UpdateInventory(_index, _item);
    }

    public void UpdateEquipments(int _index, Item item)
    {
        playerInventory.UpdateEquipments(_index, item);
    }

    public void FollowMouse()
    {
        playerInventory.FollowMouse();
    }

    public void ShowMouseHover(bool _bool, SOItems _item = null, Vector3 _pos = default)
    {
        playerInventory.ShowMouseHover(_bool, _item, _pos);
    }

    public void SetDragSprite(Sprite _sprite)
    {
        playerInventory.SetDragSprite(_sprite);
    }

    public void SetBaseDragItem(Item _item)
    {
        playerInventory.BaseDragItem = _item;
    }

    public Item GetBaseDragItem()
    {
        return playerInventory.BaseDragItem;
    }

    public int GetNbHandSlots()
    {
        return playerInventory.NbHandSlots;
    }

    public Transform GetRightHand()
    {
        return playerInventory.RightHand;
    }

    public List<Item> GetHandSlotsItems()
    {
        return playerInventory.GetHandSlotsItems();
    }

    public void SetHandSlotsItems(List<Item> _items)
    {
        playerInventory.SetHandSlotsItems(_items);
    }

    public void OnClickInventorySlot(int _index, Item _item, Vector3 _pos)
    {
        Transform divider = playerInventory.Divider;

        playerInventory.HideDivider();
        playerInventory.ShowDivider(_index, _pos);
        divider.GetChild(0).GetComponent<Slider>().onValueChanged.AddListener(delegate { playerInventory.OnSliderValueChange(); });
        divider.GetChild(1).GetComponent<Button>().onClick.AddListener(() => playerInventory.ConfirmDivide());
        divider.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ItemManager.Instance.DropItem(_item.itemType, _item.stacks, new Vector3(transform.position.x, transform.position.y - GetComponent<CharacterController>().height / 2f, transform.position.z + 1f), Quaternion.identity));
        divider.GetChild(2).GetComponent<Button>().onClick.AddListener(() => playerInventory.HideDivider());
        divider.GetChild(2).GetComponent<Button>().onClick.AddListener(() => playerInventory.RemoveItemAtIndex(_index));
        divider.GetChild(3).GetComponent<Button>().onClick.AddListener(() => playerInventory.HideDivider());
    }

    public void CleanInventory()
    {
        playerInventory.CleanInventory();
    }
    #endregion

    #region CameraMovements
    public bool GetIsInMenu()
    {
        return cameraMovements.IsInMenu;
    }

    public void SetIsInMenu(bool _isInMenu)
    {
        cameraMovements.IsInMenu = _isInMenu;
    }

    public void SetSensX(float _sensX)
    {
        cameraMovements.SensX = _sensX;
    }

    public void SetSensY(float _sensY)
    {
        cameraMovements.SensY = _sensY;
    }
    #endregion

    #region HotBarSelection

    public void AddHandItem(int _index, Item _item, GameObject _object = null)
    {
        hotBarSelection.AddHandItem(_index, _item, _object);
    }

    public void RemoveHandItem(int _index)
    {
        hotBarSelection.RemoveHandItem(_index);
    }

    public Item GetToolItem(int _index)
    {
        return hotBarSelection.toolItem[_index];
    }
    #endregion

    #region HealthComponent
    public int GetCurrentHealthPoint()
    {
        return healthComponent.currentHealthPoint;
    }

    public int GetMaxHealthPoint()
    {
        return healthComponent.maxHealthPoint;
    }

    public void HitPlayer(int _amount)
    {
        healthComponent.HitPlayer(_amount);
    }

    public void SetHasArmor(bool _bool)
    {
        healthComponent.HasArmor = _bool;
    }

    public void SetRegenTickCooldown(float _regenTickCooldown)
    {
        healthComponent.RegenTickCooldown = _regenTickCooldown;
    }
    #endregion

    #region PlayerInterract
    public void OpenCloseBuildingMenu()
    {
        playerInterract.OpenCloseBuildingMenu();
    }
    #endregion

    #region PlayerSound

    public void PlayRandomSoundInPickUp()
    {
        playerSound.PlayRandomSoundInPickUp();
    }
    #endregion

    #region PlayerScanner

    public SORessource GetSelectedResource()
    {
        return playerScanner.SelectedResource;
    }

    public void SetSelectedResource(SORessource _resource)
    {
        playerScanner.SelectedResource = _resource;
    }

    public void SpawnPing(Collider _other)
    {
        playerScanner.SpawnPing(_other);
    }

    public void AddScannableResource(int _index)
    {
        playerScanner.AddScannableResource(_index);
    }

    #endregion

    #region Player Buildance
    public void SetHUBPlaced(bool _bool)
    {
        playerBuildance.HUBPlaced = _bool;
    }

    public void SetRCPlaced(bool _bool)
    {
        playerBuildance.RCPlaced = _bool;
    }

    public bool GetOnDestructionMode()
    {
        return playerBuildance.onDestructionMode;
    }
    #endregion

    #region PlayerAnimation

    public void SetBoolAnimator(string _name, bool _value)
    {
        playerAnimation.SetBool(_name, _value);
    }

    public void SetTriggerAnimator(string _name)
    {
        playerAnimation.SetTrigger(_name);
    }

    #endregion

    #region Maths
    public float Remap(float value, float min1, float max1, float min2, float max2)
    {
        return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
    }
    #endregion

    #region Cheat Engine
    public void SetSpeedMultiplier(float value)
    {
        playerMovements.SpeedMultiplier = value;
    }
    public void SetJumpMultiplier(float value)
    {
        playerMovements.JumpMultiplier = value;
    }
    public void SetDamageMultiplier(float value) { }
    public void SetInvincible(bool value)
    {
        healthComponent.Invincible = value;
    }
    public void FullRegen()
    {
        healthComponent.FullRegen();
        healthUI.ResetHealthBar();
    }
    #endregion
}