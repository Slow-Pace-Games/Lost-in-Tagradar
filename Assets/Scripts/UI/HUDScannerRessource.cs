using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDScannerRessource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public SORessource resource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverImage(new Color(1f, 1f, 1f, 0.7f));
        Player.Instance.SetSelectedResource(resource);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverImage(new Color(1f, 1f, 1f, 1f));
        Player.Instance.SetSelectedResource(null);
    }

    public void OnHoverImage(Color _newColor)
    {
        GetComponent<Image>().color = _newColor;
    }
}