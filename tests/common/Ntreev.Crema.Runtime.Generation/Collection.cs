using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Ntreev.Crema.Runtime.Generation
{
    [CollectionDefinition("Ntreev.Crema.Runtime.Generation")]
    public class Collection
    {
    }
}
