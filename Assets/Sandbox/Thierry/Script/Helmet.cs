using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helmet : MonoBehaviour
{
    [SerializeField] private Image helmetTransition;
    private Image helmetImage;
    [SerializeField] List<Sprite> animationHelmetTransitionNormal;
    [SerializeField] List<Sprite> animationHelmetTransitionBuild;
    [SerializeField] List<Sprite> animationHelmetTransitionDestruction;
    [SerializeField] Sprite HelmetExploration;
    [SerializeField] Sprite HelmetDestruction;
    [SerializeField] Sprite HelmetConstruction;


    private float speed = 0.05f;
    public void Start()
    {
        helmetImage = GetComponent<Image>();
    }
    public void TransitionExploration()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationController.PlayAnimation(animationHelmetTransitionNormal, helmetTransition, false, speed));
        helmetImage.sprite = HelmetExploration;
    }
    public void TransitionBuild()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationController.PlayAnimation(animationHelmetTransitionBuild, helmetTransition, false, speed));
        helmetImage.sprite = HelmetConstruction;
    }

    public void TransitionDestruction() 
    {
        StopAllCoroutines();
        StartCoroutine(AnimationController.PlayAnimation(animationHelmetTransitionDestruction, helmetTransition, false, speed));
        helmetImage.sprite = HelmetDestruction;
    }
}
