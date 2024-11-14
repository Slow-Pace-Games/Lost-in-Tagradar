
public class OrganizationPanel : BuildMenuMiddlePanel, IEditablePanel
{
    public void InitPanel()
    {
        InitMachinesContainer("Organization", MachineClass.Organization);
    }
}
