using UnityEngine;
using UnityEngine.EventSystems;

public class CraftButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] CraftBench craftbench;

    private bool isInButton = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInButton = false;
    }

    private void Update()
    {
        if (PlayerInputManager.Instance.IsCraftMousePressed() && isInButton)
        {
            craftbench.Prod();
        }
    }
}
