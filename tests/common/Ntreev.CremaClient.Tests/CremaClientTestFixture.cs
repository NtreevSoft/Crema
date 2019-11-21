using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Ntreev.CremaServer.Tests;

namespace Ntreev.CremaClient.Tests
{
    public class CremaClientTestFixture : IDisposable
    {
        public ICremaHost CremaHost { get; set; }
        public CremaServerTestFixture ServerHost { get; set; }

        public CremaClientTestFixture()
        {
            this.ServerHost = new CremaServerTestFixture();

            var bootstrap = new CremaBootstrapper();
            this.CremaHost = (ICremaHost)bootstrap.GetService(typeof(ICremaHost));

        }

        public void Dispose()
        {
            CremaHost?.Dispose();
            ServerHost?.Dispose();
        }
    }
}
