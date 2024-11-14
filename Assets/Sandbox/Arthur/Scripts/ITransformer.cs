using System.Collections.Generic;

public interface ITransformer
{
    enum Resource
    {
        Steel,
        Iron,
        Gold
    }

    void TransformResources(params Resource[] _resources)
    {

    }
}
