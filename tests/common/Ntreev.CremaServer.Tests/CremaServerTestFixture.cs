//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Reflection;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Extensions;
using Ntreev.Library.IO;

namespace Ntreev.CremaServer.Tests
{
    public class CremaServerTestFixture : IDisposable
    {
        protected CremaService Service { get; private set; }
        public ICremaHost CremaHost { get; private set; }

        public static readonly string CremaDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());

        public CremaServerTestFixture()
        {
            InitAppUtility();
            InitEntryAssembly();
            InitCremaService();
        }

        private void InitCremaService()
        {
            var repoPath = DirectoryUtility.Prepare(CremaDbPath, "_repo");
            using (var bootstrap = new CremaBootstrapper())
            {
                bootstrap.CreateRepository(repoPath, "crema", false);
            }

            Service = new CremaService();
            Service.BasePath = repoPath;
            Service.MultiThreading = true;
            Service.Port = 4004;
            Service.HttpPort = AddressUtility.GetDefaultHttpPort(Service.Port);

            Service.Open();

            CremaHost = Service.GetService<ICremaHost>();
        }

        private static void InitAppUtility()
        {
            AppUtility.ProductName = "crema";
            AppUtility.ProductVersion = "3.6.0.0";
        }

        private static void InitEntryAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            var manager = new AppDomainManager();
            var entryAssemblyField = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyField?.SetValue(manager, assembly);

            var domain = AppDomain.CurrentDomain;
            var domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField?.SetValue(domain, manager);
        }

        public void Dispose()
        {
            Service?.Close();
            Service?.Dispose();
        }
    }
}
