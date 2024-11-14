using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color hoverColor;
    private Color normalColor;
    Image image;

    private void Start()
    {
        hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        normalColor = new Color(1f, 1f, 1f, 1f);
        image = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Player.Instance.SetBaseDragItem(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }
}
