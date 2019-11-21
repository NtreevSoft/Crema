﻿using Ntreev.CremaServer.Tests;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Ntreev.Crema.ServiceHosts.Http.Tests
{
    [CollectionDefinition("Ntreev.Crema.Services.Tests")]
    public class Collection : ICollectionFixture<CremaServerTestFixture>
    {
    }
}
