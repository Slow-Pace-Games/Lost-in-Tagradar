using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodexObjectTitleInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    Color initColor;

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        initColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color newColor = new Color(initColor.r, initColor.g, initColor.b, 20f / 255f);
        image.color = newColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = initColor;
    }

}
