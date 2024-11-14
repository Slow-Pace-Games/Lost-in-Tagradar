using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    Compass compass;
    [SerializeField] ShortCutBuildScript ShortCutBuild;
    Helmet helmet;
    ShortCutScript shortCut;
    PingJalon pingJalon;
    HandItemUI handItemUI;
    HotBarUi hotBarUI;
    [SerializeField] ScanUi scanUi;
    [SerializeField] GameObject crossAir;
    [SerializeField] Image progressionMinage;
    [SerializeField] InteractionMessage interactionMessage;

    #region Singleton
    private static PlayerUi instance;
    public static PlayerUi Instance { get => instance; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    void Init()
    {
        compass = GetComponentInChildren<Compass>();
        helmet = GetComponentInChildren<Helmet>();
        shortCut = GetComponentInChildren<ShortCutScript>();
        pingJalon = GetComponentInChildren<PingJalon>();
        handItemUI = GetComponentInChildren<HandItemUI>();
        hotBarUI = GetComponentInChildren<HotBarUi>();
        SetIsJalonActive(false);
    }

    private void Update()
    {
        compass.UpdateCompass();
    }

    public void SwapInConstructionMode()
    {
        ShortCutBuild.gameObject.SetActive(true);
        helmet.TransitionBuild();
        LoidUI.Instance.MoveDialogue(MoveDirection.Up);
    }

    public void UpdateFillAmountMinage(float amount)
    {
        progressionMinage.fillAmount = amount;
    }
    public void SwapInExplorationMode()
    {
        ShortCutBuild.gameObject.SetActive(false);
        helmet.TransitionExploration();
        LoidUI.Instance.MoveDialogue(MoveDirection.Down);
    }

    public void SwapInDestructionMode()
    {
        ShortCutBuild.gameObject.SetActive(false);
        helmet.TransitionDestruction();
        LoidUI.Instance.MoveDialogue(MoveDirection.Down);
    }

    #region shortCutBuild
    public void ShortCutBuildUpdate(SOBuildingData building)
    {
        ShortCutBuild.gameObject.SetActive(true);
        ShortCutBuild.UpdateShortCutBuild(building);
    }
    public void ShortCutBuildUpdateCost()
    {
        ShortCutBuild.UpdateCost();
    }

    public void DisableShortCutBuild()
    {
        ShortCutBuild.gameObject.SetActive(false);
    }
    #endregion

    #region ShortCut
    public void UpdateShortCut()
    {
        shortCut.UpdateText();
    }
    #endregion

    #region HandHotbar
    public void UpdateHandHotBar(string name, Sprite icon)
    {
        handItemUI.SetHandInfo(name, icon);
    }
    public void HideHandUi()
    {
        handItemUI.HideUI();
    }
    public void SelectItemHand()
    {
        handItemUI.Select();
    }
    public void UnselectItemHand()
    {
        handItemUI.Unselect();
    }
    public void SetIsJalonActive(bool _isActive)
    {
        pingJalon.gameObject.SetActive(_isActive);
    }
    #endregion

    #region Jalon
    public List<IconItemWithQuantity> GetPingJalonItems()
    {
        return pingJalon.items;
    }

    public TextMeshProUGUI GetPingJalonTitle()
    {
        return pingJalon.title;
    }

    public void UpdateQuantityInInventory()
    {
        pingJalon.UpdateQuantityInInventory();
    }

    public void UnselectAllItemHotbar()
    {
        hotBarUI.UnselectAllItem();
    }
    #endregion

    #region InteractionMessage

    public void UpdateKeyNamesInteraction()
    {
        interactionMessage.UpdateKeyNames();
    }

    #endregion

    public void AddItemInHotBar(Sprite image, int index)
    {
        hotBarUI.AddItem(image, index);
    }

    #region scan
    public void OpenScanUi()
    {
        scanUi.OpenRessourceSelectionMenu();
    }
    public void CloseScanUi()
    {
        scanUi.CloseRessourceSelectionMenu();
    }
    #endregion
}
