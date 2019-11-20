using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.CremaServer.Tests;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Ntreev.Crema.Services.Tests
{
    [CollectionDefinition("Ntreev.Crema.Services.Tests")]
    public class Collection : ICollectionFixture<CremaServerTestFixture>
    {
    }
}
