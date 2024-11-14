using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScanPing : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI lengthText;

    private Transform deposit;
    private Transform player;
    private RectTransform uiTransform;

    private const float maxAngle = 360f;
    private const int compassLengthPx = 2048;

    void Update()
    {
        //pos ping in canvas
        Vector3 vectorPlayerDeposit = deposit.position - player.position;
        float signAngle = Mathf.Sign(Vector3.Cross(player.forward, vectorPlayerDeposit).y);
        float angle = Vector3.Angle(player.forward, vectorPlayerDeposit) * signAngle;
        float angleOffset = angle / maxAngle;
        float percent = angleOffset + 0.5f;
        float xPos = percent * compassLengthPx % compassLengthPx;
        uiTransform.anchoredPosition = new Vector2(xPos, uiTransform.anchoredPosition.y);

        //meter distance
        lengthText.text = ((int)vectorPlayerDeposit.magnitude).ToString() + "m";
    }

    public void InitPing(Transform deposit, Transform player, Sprite itemSprite)
    {
        uiTransform = GetComponent<RectTransform>();
        GetComponentInChildren<Image>().sprite = itemSprite;

        this.deposit = deposit;
        this.player = player;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(deposit.position, player.position);
        Handles.DrawAAPolyLine(player.position, player.position + player.forward * 15f);
    }
#endif
}