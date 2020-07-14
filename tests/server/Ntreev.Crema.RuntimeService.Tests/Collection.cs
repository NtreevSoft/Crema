using Ntreev.CremaServer.Tests;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Ntreev.Crema.RuntimeService.Tests
{
    [CollectionDefinition("Ntreev.Crema.RuntimeService.Tests")]
    public class Collection : ICollectionFixture<CremaServerTestFixture>
    {
    }
}
