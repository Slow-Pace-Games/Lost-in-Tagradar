public class TutoContainer : SaveContainer
{
    protected override void Convert()
    {
        SaveSystem.Instance.ClassContainer.tutoProgress = Loid.Instance.GetTutoPhase();
    }

    protected override void Load()
    {
        Loid.Instance.SetTutoPhase(SaveSystem.Instance.ClassContainer.tutoProgress);
    }
}