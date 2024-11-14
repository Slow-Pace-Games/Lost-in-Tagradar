
public class LogisticsPanel : BuildMenuMiddlePanel, IEditablePanel
{
    public void InitPanel()
    {
        InitMachinesContainer("Logistics", MachineClass.Logistics);
    }
}
