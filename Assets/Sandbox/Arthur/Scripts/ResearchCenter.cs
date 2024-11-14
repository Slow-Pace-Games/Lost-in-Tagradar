using UnityEngine;

public class ResearchCenter : UniqueBuilding, IMachineInteractable
{
    [Header("Canvas")]
    [SerializeField] private RCCanvas rcCanvas;

    #region IMachineInteractable
    public void OpenWindow()
    {
        rcCanvas.OpenRCCanvas();
    }
    public void CloseWindow()
    {
        rcCanvas.CloseRCCanvas();
    }
    #endregion

    #region IDestructible
    public override void Destruct()
    {
        Player.Instance.SetRCPlaced(false);
        base.Destruct();
    }
    #endregion
}
