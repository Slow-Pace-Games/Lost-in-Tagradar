using UnityEngine;

public static class TimeScale
{
    private static float timeMultiplier = 1f;
    public static float TimeMultiplier { set => timeMultiplier = value; }

    public static float deltaTime { get => Time.deltaTime * timeMultiplier; }
    public static float fixedTime { get => Time.fixedTime * timeMultiplier; }
    public static float unscaledTime { get => Time.unscaledTime * timeMultiplier; }
}