using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayCheat : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private Slider timeMultiplier;
    [SerializeField] private TextMeshProUGUI valueTimeMultiplier;
    [SerializeField] private Toggle spawnMob;
    [SerializeField] private Button reset;

    private float defaultTimeMultiplier = 1f;
    private bool defaultSpawnMob = true;

    private void Start()
    {
        timeMultiplier.value = defaultTimeMultiplier;
        timeMultiplier.onValueChanged.AddListener(UpdateTimeMultiplier);

        spawnMob.isOn = defaultSpawnMob;
        //spawnMob.onValueChanged.AddListener();

        reset.onClick.AddListener(ResetToDefaultValue);
    }

    private void ResetToDefaultValue()
    {
        timeMultiplier.value = defaultTimeMultiplier;
        spawnMob.isOn = defaultSpawnMob;
    }

    private void UpdateTimeMultiplier(float value)
    {
        TimeScale.TimeMultiplier = value;
        valueTimeMultiplier.text = "x" + value.ToString("F1");
    }
}