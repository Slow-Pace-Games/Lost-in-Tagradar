using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpaceshipCanvas : MonoBehaviour
{
    #region Singleton
    private static SpaceshipCanvas instance;
    public static SpaceshipCanvas Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
    #endregion

    [Header("Menus")]
    [SerializeField] private List<RepairMenu> menu;
    [SerializeField] private LaunchSpaceshipMenu launchSpaceshipMenu;

    [Header("Mesh Parts Transform")]
    [SerializeField] private Transform[] spaceshipDestroyMeshTransforms = new Transform[4];
    [SerializeField] private Transform[] spaceshipRepairedMeshTransforms = new Transform[4];

    [Header("Virtuals Cams Transform")]
    [SerializeField] private Transform[] vCams = new Transform[4];

    [Header("Buttons Menu")]
    [SerializeField] private Transform buttonsContainerTransform;
    [SerializeField] private Button[] buttonsMenu = new Button[4];
    [SerializeField] private Sprite[] spritesSelectedRepairedButtons = new Sprite[4];
    [SerializeField] private Sprite[] spritesSelectedBrokenButtons = new Sprite[4];
    [SerializeField] private Sprite[] spritesNotSelectedRepairedButtons = new Sprite[4];
    [SerializeField] private Sprite[] spritesNotSelectedBrokenButtons = new Sprite[4];

    //Delegates
    public delegate void OnNbPartsRepairedIncrease();
    public OnNbPartsRepairedIncrease onNbPartsRepairedIncrease;
    public delegate void OnAllPartsRepaired();
    public OnAllPartsRepaired onAllPartsRepaired;
    public delegate void OnLaunchSpaceship();
    public OnLaunchSpaceship onLaunchSpaceship;

    enum CurrentMenu
    {
        BODY,
        COCKPIT,
        TOP_WINGS,
        BOTTOM_WINGS,
        NONE
    }

    [SerializeField] private CurrentMenu currentMenu;
    private bool areAllPartsRepaired = false;

    private void Start()
    {
        for (int i = 0; i < menu.Count; i++)
        {
            if (menu[i].isRepaired)
            {
                spaceshipRepairedMeshTransforms[i].gameObject.SetActive(true);
                spaceshipDestroyMeshTransforms[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < menu.Count; i++)
        {
            menu[i].Init();
        }

        currentMenu = CurrentMenu.NONE;

        for (int i = 0; i < menu.Count; i++)
        {
            SwitchSpriteButton(buttonsMenu[i], i);
        }
    }

    #region Open/Closing Canvas
    public void OpenSpaceshipCanvas()
    {
        if (!areAllPartsRepaired)
        {
            buttonsContainerTransform.gameObject.SetActive(true);
            OpenBodyMenu();
            onNbPartsRepairedIncrease += IsAllPartsRepaired;
        }
        else
        {

            launchSpaceshipMenu.gameObject.SetActive(true);
            launchSpaceshipMenu.OpenLaunchSpaceshipMenu();
        }
    }

    IEnumerator WaitCameraStopMoving()
    {
        float time = 0f;
        while (time < 2f)
        {
            time += TimeScale.deltaTime;
            PlayerInputManager.Instance.DisableCamera();
            PlayerInputManager.Instance.DisableMovement();
            yield return null;
        }

        PlayerInputManager.Instance.EnableCamera();
        PlayerInputManager.Instance.EnableMovement();
    }

    public void CloseSpaceshipCanvas()
    {
        if (!areAllPartsRepaired || !launchSpaceshipMenu.gameObject.activeSelf)
        {
            ClosingLastMenu();
            buttonsContainerTransform.gameObject.SetActive(false);
            currentMenu = CurrentMenu.NONE;
            onNbPartsRepairedIncrease -= IsAllPartsRepaired;
        }
        else
        {
            launchSpaceshipMenu.CloseLaunchSpaceshipMenu();
            launchSpaceshipMenu.gameObject.SetActive(false);
        }
        StartCoroutine(WaitCameraStopMoving());
    }

    public void CloseSpaceshipCanvasWithCross()
    {
        Player.Instance.OpenCloseBuildingMenu();
        EventSystem.current.SetSelectedGameObject(null);
    }
    #endregion

    #region Open/Closing Menu
    private void ClosingLastMenu()
    {
        if (currentMenu != CurrentMenu.NONE)
        {
            buttonsMenu[(int)currentMenu].interactable = true;
            menu[(int)currentMenu].gameObject.SetActive(false);
            vCams[(int)currentMenu].gameObject.SetActive(false);

        }
    }

    public void OpenBodyMenu()
    {
        ClosingLastMenu();
        menu[(int)CurrentMenu.BODY].OpenRepairMenu();
        currentMenu = CurrentMenu.BODY;
        for (int i = 0; i < menu.Count; i++)
        {
            SwitchSpriteButton(buttonsMenu[i], i);
        }
        buttonsMenu[(int)currentMenu].interactable = false;
        vCams[(int)currentMenu].gameObject.SetActive(true);
    }

    public void OpenCockpitMenu()
    {
        ClosingLastMenu();

        menu[(int)CurrentMenu.COCKPIT].OpenRepairMenu();
        currentMenu = CurrentMenu.COCKPIT;
        for (int i = 0; i < menu.Count; i++)
        {
            SwitchSpriteButton(buttonsMenu[i], i);
        }
        buttonsMenu[(int)currentMenu].interactable = false;
        vCams[(int)currentMenu].gameObject.SetActive(true);
    }

    public void OpenTopWingsMenu()
    {
        ClosingLastMenu();

        menu[(int)CurrentMenu.TOP_WINGS].OpenRepairMenu();
        currentMenu = CurrentMenu.TOP_WINGS;
        for (int i = 0; i < menu.Count; i++)
        {
            SwitchSpriteButton(buttonsMenu[i], i);
        }
        buttonsMenu[(int)currentMenu].interactable = false;
        vCams[(int)currentMenu].gameObject.SetActive(true);
    }

    public void OpenBottomWingsMenu()
    {
        ClosingLastMenu();

        menu[(int)CurrentMenu.BOTTOM_WINGS].OpenRepairMenu();
        currentMenu = CurrentMenu.BOTTOM_WINGS;
        for (int i = 0; i < menu.Count; i++)
        {
            SwitchSpriteButton(buttonsMenu[i], i);
        }
        buttonsMenu[(int)currentMenu].interactable = false;
        vCams[(int)currentMenu].gameObject.SetActive(true);
    }
    #endregion

    public void IsAllPartsRepaired()
    {
        spaceshipRepairedMeshTransforms[(int)currentMenu].gameObject.SetActive(true);
        spaceshipDestroyMeshTransforms[(int)currentMenu].gameObject.SetActive(false);

        SwitchSpriteButton(buttonsMenu[(int)currentMenu], (int)currentMenu);

        for (int i = 0; i < menu.Count; i++)
        {
            if (!menu[i].isRepaired)
            {
                return;
            }
        }
        areAllPartsRepaired = true;
        ClosingLastMenu();
        buttonsContainerTransform.gameObject.SetActive(false);
        currentMenu = CurrentMenu.NONE;
        launchSpaceshipMenu.gameObject.SetActive(true);
        launchSpaceshipMenu.OpenLaunchSpaceshipMenu();
        onAllPartsRepaired?.Invoke();
    }

    private void SwitchSpriteButton(Button button, int index)
    {
        SpriteState tempSpriteState;

        if ((int)currentMenu == index)
        {
            if (menu[index].isRepaired)
            {
                button.image.sprite = spritesNotSelectedRepairedButtons[index];
                tempSpriteState.selectedSprite = spritesSelectedRepairedButtons[index];
                tempSpriteState.highlightedSprite = spritesSelectedRepairedButtons[index];
                tempSpriteState.pressedSprite = spritesSelectedRepairedButtons[index];
                tempSpriteState.disabledSprite = spritesSelectedRepairedButtons[index];
            }
            else
            {
                tempSpriteState.selectedSprite = spritesSelectedBrokenButtons[index];
                tempSpriteState.highlightedSprite = spritesSelectedBrokenButtons[index];
                tempSpriteState.pressedSprite = spritesSelectedBrokenButtons[index];
                tempSpriteState.disabledSprite = spritesSelectedBrokenButtons[index];
            }
        }
        else
        {
            if (menu[index].isRepaired)
            {
                button.image.sprite = spritesNotSelectedRepairedButtons[index];
                tempSpriteState.selectedSprite = spritesNotSelectedRepairedButtons[index];
                tempSpriteState.highlightedSprite = spritesSelectedRepairedButtons[index];
                tempSpriteState.pressedSprite = spritesNotSelectedRepairedButtons[index];
                tempSpriteState.disabledSprite = spritesNotSelectedRepairedButtons[index];
            }
            else
            {
                tempSpriteState.selectedSprite = spritesNotSelectedBrokenButtons[index];
                tempSpriteState.highlightedSprite = spritesSelectedBrokenButtons[index];
                tempSpriteState.pressedSprite = spritesNotSelectedBrokenButtons[index];
                tempSpriteState.disabledSprite = spritesNotSelectedBrokenButtons[index];
            }
        }

        button.spriteState = tempSpriteState;
    }

    #region Function For The Save
    public bool[] ArePartsRepaired()
    {
        bool[] allPartsBool = new bool[4];

        for (int i = 0; i < menu.Count; i++)
        {
            allPartsBool[i] = menu[i].isRepaired;
        }
        return allPartsBool;
    }

    public void SetPartsRepaired(bool[] arePartsRepaired)
    {
        for (int i = 0; i < menu.Count; i++)
        {
            menu[i].isRepaired = arePartsRepaired[i];
            if (arePartsRepaired[i])
            {
                menu[i].Repair(false);
                spaceshipRepairedMeshTransforms[i].gameObject.SetActive(true);
                spaceshipDestroyMeshTransforms[i].gameObject.SetActive(false);
                SwitchSpriteButton(buttonsMenu[i], i);
            }
        }
    }
    #endregion
}