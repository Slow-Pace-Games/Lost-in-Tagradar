using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] List<PlayableDirector> timeline;

    private void Start()
    {
        SpaceshipCanvas.Instance.onLaunchSpaceship += () => StartTimeline(0);
        PlayerInputManager.Instance.StartTravelingAction(() => StartTimeline(1), PlayerInputManager.ActionType.Add);
    }

    private void StartTimeline(int index) => timeline[index].Play();
}