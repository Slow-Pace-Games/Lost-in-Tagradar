using UnityEngine;

public class HUB : UniqueBuilding, IMachineInteractable
{
    [Header("Canvas")]
    [SerializeField] private HUBCanvas hubCanvas;

    #region IMachineInteractable
    public void OpenWindow()
    {
        hubCanvas.OpenHUBCanvas();
    }
    public void CloseWindow()
    {
        hubCanvas.CloseHUBCanvas();
    }
    #endregion

    #region IBuildable
    public override void Build()
    {
        base.Build();
        Loid.Instance.UpdateTuto(PlayerAction.PlaceHub);
    }

    #endregion

    #region IDestructible
    public override void Destruct()
    {
        Player.Instance.SetHUBPlaced(false);
        base.Destruct();
    }
    #endregion
}