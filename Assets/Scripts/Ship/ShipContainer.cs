public class ShipContainer : SaveContainer
{
    protected override void Convert()
    {
        SaveSystem.Instance.ClassContainer.shipProgress = SpaceshipCanvas.Instance.ArePartsRepaired();
    }

    protected override void Load()
    {
        SpaceshipCanvas.Instance.SetPartsRepaired(SaveSystem.Instance.ClassContainer.shipProgress);
    }
}