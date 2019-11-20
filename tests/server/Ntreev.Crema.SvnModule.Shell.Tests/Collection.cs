using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.CremaServer.Tests;
using Xunit;

namespace Ntreev.Crema.SvnModule.Shell.Tests
{
    [CollectionDefinition("Ntreev.Crema.SvnModule.Shell.Tests")]
    public class Collection : ICollectionFixture<CremaServerTestFixture>
    {
    }
}
