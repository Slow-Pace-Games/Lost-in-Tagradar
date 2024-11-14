using System.Collections.Generic;

public interface IAttachement
{
    public List<Attachement> GetInputAttachements();
    public List<Attachement> GetOutputAttachements();
}
