
public class ProductionPanel : BuildMenuMiddlePanel, IEditablePanel
{
    public void InitPanel()
    {
        InitMachinesContainer("Production", MachineClass.Production);
    }
}
