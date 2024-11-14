
public class SpecialPanel : BuildMenuMiddlePanel, IEditablePanel
{
    public void InitPanel()
    {
        InitMachinesContainer("Special", MachineClass.Special);
    }
}
