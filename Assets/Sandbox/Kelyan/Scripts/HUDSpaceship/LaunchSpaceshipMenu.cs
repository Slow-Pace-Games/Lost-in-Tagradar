using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchSpaceshipMenu : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [Header("Sprites lists")]
    [SerializeField] private List<Sprite> openAnimSprites;
    [SerializeField] private List<Sprite> leverSprites;

    [Header("Images To Anim")]
    [SerializeField] private Image lever;
    [SerializeField] private Image openMenu;

    private bool launchSpaceship = false;
    private bool mouseButtonLeftReleased = false;
    private bool isOpenAnimationFinished = false;

    public void OpenLaunchSpaceshipMenu()
    {
        if (!isOpenAnimationFinished)
        {
            StartCoroutine(AnimationController.PlayAnimation(openAnimSprites, openMenu, false, 0.050f));
        }
    }

    public void CloseLaunchSpaceshipMenu()
    {
        isOpenAnimationFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpenAnimationFinished && openMenu.sprite == openAnimSprites[openAnimSprites.Count - 1])
        {
            isOpenAnimationFinished = true;
        } 

        if (!launchSpaceship && isOpenAnimationFinished)
        {
            CheckMouseButtonState();
            if (slider.gameObject.activeSelf && slider.value < 1 && mouseButtonLeftReleased)
            {
                mouseButtonLeftReleased = true;
                slider.value -= Time.deltaTime * 5;

            }
            else if (slider.value == 1)
            {
                slider.interactable = false;
                launchSpaceship = true;
                SpaceshipCanvas.Instance.onLaunchSpaceship?.Invoke();
            }
            AnimationController.PlayAnimationAssignedValue(leverSprites, lever, slider.value);
        }
    }

    private void CheckMouseButtonState()
    {
        if (Input.GetMouseButtonUp(0))
        {
            mouseButtonLeftReleased = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            mouseButtonLeftReleased = false;
        }
    }

}
