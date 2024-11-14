using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] RawImage image;

    public void UpdateCompass()
    {
        Player player = Player.Instance;
        float playerAngle = player.transform.localEulerAngles.y;
        if (playerAngle < 0)
        {
            playerAngle = player.Remap(playerAngle, -180f, 0f, 180f, 360f);
        }
        image.uvRect = new Rect(playerAngle / 360f, 0f, 1f, 1f);
    }
}
