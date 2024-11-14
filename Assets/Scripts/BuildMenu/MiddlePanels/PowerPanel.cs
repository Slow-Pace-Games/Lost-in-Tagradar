
public class PowerPanel : BuildMenuMiddlePanel, IEditablePanel
{
    public void InitPanel()
    {
        InitMachinesContainer("Power", MachineClass.Power);
    }
}
