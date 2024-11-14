using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour
{
    [SerializeField] UnityEngine.Sprite initialSprite;
    [SerializeField] UnityEngine.Sprite hoverSprite;

    UnityEngine.Sprite actualSprite;
    Image actualImage;
    void Start()
    {
        actualImage = GetComponent<Image>();
    }

    // Appelé lorsque la souris entre dans la zone du bouton.
    public void OnPointerEnter(PointerEventData eventData)
    {
        actualImage.sprite = hoverSprite;
    }

    // Appelé lorsque la souris quitte la zone du bouton.
    public void OnPointerExit(PointerEventData eventData)
    {
        actualImage.sprite = initialSprite;
    }
}
