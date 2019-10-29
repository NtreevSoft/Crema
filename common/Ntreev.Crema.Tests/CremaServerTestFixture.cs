using System;
using System.IO;
using System.Reflection;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Extensions;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Tests
{
    public class CremaServerTestFixture
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
            AppUtility.ProductVersion = "3.7.0.0";
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
