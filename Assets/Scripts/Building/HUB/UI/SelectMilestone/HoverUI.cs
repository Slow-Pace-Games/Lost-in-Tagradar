using UnityEngine.EventSystems;
using UnityEngine;

public class HoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected HoverData hoverData;
    public virtual HoverData HoverData
    {
        set
        {
            hoverData = value;
        }

        get
        {
            return hoverData;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => HUBCanvas.Instance.UpdateHoverSelectMilestone(hoverData);
    public void OnPointerExit(PointerEventData eventData) => HUBCanvas.Instance.ExitHoverSelectMilestone();
}

public struct HoverData
{
    public readonly Sprite icon;
    public readonly string name;
    public readonly bool haveValue;
    public readonly int value;

    public HoverData(Sprite icon, string name)
    {
        this.icon = icon;
        this.name = name;

        haveValue = false;
        value = 0;
    }

    public HoverData(Sprite icon, string name, int value)
    {
        this.icon = icon;
        this.name = name;

        haveValue = true;
        this.value = value;
    }
}