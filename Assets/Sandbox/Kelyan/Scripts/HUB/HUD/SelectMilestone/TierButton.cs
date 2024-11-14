using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI tierName;

    public Button Button { get => button; }

    public void Init(Sprite icon, string tierName)
    {
        this.icon.sprite = icon;
        this.tierName.text = tierName;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}