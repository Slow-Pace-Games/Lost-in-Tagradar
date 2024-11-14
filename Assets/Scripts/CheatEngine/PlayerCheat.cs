using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCheat : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Slider speedMultiplier;
    [SerializeField] private TextMeshProUGUI textSpeedMultiplier;
    [SerializeField] private Slider jumpMultiplier;
    [SerializeField] private TextMeshProUGUI textJumpMultiplier;
    [SerializeField] private Slider damageMultiplier;
    [SerializeField] private TextMeshProUGUI textDamageMultiplier;
    [SerializeField] private Button regen;
    [SerializeField] private Toggle invincible;
    [SerializeField] private Button reset;

    [Header("Default Value")]
    private float defaultSpeedMultiplier = 1f;
    private float defaultJumpMultiplier = 1f;
    private float defaultDamageMultiplier = 1f;
    private bool defaultInvincible = false;

    private void Start()
    {
        speedMultiplier.value = defaultSpeedMultiplier;
        speedMultiplier.onValueChanged.AddListener(Player.Instance.SetSpeedMultiplier);
        speedMultiplier.onValueChanged.AddListener((value) => textSpeedMultiplier.text = "x" + value.ToString("F1"));

        jumpMultiplier.value = defaultJumpMultiplier;
        jumpMultiplier.onValueChanged.AddListener(Player.Instance.SetJumpMultiplier);
        jumpMultiplier.onValueChanged.AddListener((value) => textJumpMultiplier.text = "x" + value.ToString("F1"));

        damageMultiplier.value = defaultDamageMultiplier;
        damageMultiplier.onValueChanged.AddListener(Player.Instance.SetDamageMultiplier);
        damageMultiplier.onValueChanged.AddListener((value) => textDamageMultiplier.text = "x" + value.ToString("F1"));

        invincible.isOn = defaultInvincible;
        invincible.onValueChanged.AddListener(Player.Instance.SetInvincible);

        regen.onClick.AddListener(Player.Instance.FullRegen);
        reset.onClick.AddListener(ResetToDefaultValue);
    }

    private void ResetToDefaultValue()
    {
        speedMultiplier.value = defaultSpeedMultiplier;
        jumpMultiplier.value = defaultJumpMultiplier;
        damageMultiplier.value = defaultDamageMultiplier;

        invincible.isOn = defaultInvincible;
    }
}