using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Rendering")]
    [SerializeField] private TextMeshProUGUI nameRecipe;
    [SerializeField] private Image icon;

    public void Init(SORecipe recipe)
    {
        nameRecipe.text = recipe.NameRecipe;//juste un init du texte et de l'image
        icon.sprite = recipe.ItemOutput.Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MachineHUDManager.Instance.onHover?.Invoke(transform.GetSiblingIndex());//on donne la position du bouton dans la hiérarchie (même emplacment que le panel hover jsute différent container)
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MachineHUDManager.Instance.onHover?.Invoke(-1);//on quitte le boutton donc -1
    }
}