 using UnityEngine;
using UnityEngine.UI;

public class HotBarUi : MonoBehaviour
{
    [SerializeField] RectTransform[] box = new RectTransform[10];
    [SerializeField] Image[] iconBuilding = new Image[10];

    public void SelectItem(int  index)
    {
        box[index].sizeDelta = new Vector2(80f, 80f);
    }

    public void UnselectAllItem()
    {
        for (int i = 0; i < box.Length; i++)
        {
            box[i].sizeDelta = new Vector2(70f, 70f);
        }
    }

    public void AddItem(Sprite image, int index)
    {
        iconBuilding[index].sprite = image;
        iconBuilding[index].gameObject.SetActive(true);
    }
}
