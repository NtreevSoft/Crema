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

using Ntreev.Library;
using Ntreev.Crema.Services.Users;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Collections;
using Ntreev.Library.Linq;
using Ntreev.Crema.Services.Data;
using System.Security;
using System.ComponentModel.Composition.Primitives;

namespace Ntreev.Crema.Services
{
    public class CremaBootstrapper : IServiceProvider, IDisposable
    {
        public static int DefaultPort = 4004;
        public const string DefaultDataBase = "default";
        private const string pluginsString = "plugins";

        private CompositionContainer container;

        static CremaBootstrapper()
        {
#if DEBUG
            DefaultInactivityTimeout = TimeSpan.MaxValue;
#else
            DefaultInactivityTimeout = new TimeSpan(0, 1, 0);
#endif
        }

        public CremaBootstrapper()
        {
            this.Initialize();
        }

        public static bool IsOnline(string address, string userID, SecureString password)
        {
            var serviceClient = DescriptorServiceFactory.CreateServiceClient(address);
            serviceClient.Open();
            try
            {
                return serviceClient.IsOnline(userID, UserContext.Encrypt(userID, password));
            }
            finally
            {
                serviceClient.Close();
            }
        }

        public static DataBaseInfo[] GetDataBases(string address)
        {
            var serviceClient = DescriptorServiceFactory.CreateServiceClient(address);
            serviceClient.Open();
            try
            {
                return serviceClient.GetDataBaseInfos();
            }
            finally
            {
                serviceClient.Close();
            }
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;

            if (typeof(IEnumerable).IsAssignableFrom(serviceType) && serviceType.GenericTypeArguments.Length == 1)
            {
                var itemType = serviceType.GenericTypeArguments.First();
                var contractName = AttributedModelServices.GetContractName(itemType);
                var items = this.container.GetExportedValues<object>(contractName);
                var listGenericType = typeof(List<>);
                var list = listGenericType.MakeGenericType(itemType);
                var ci = list.GetConstructor(new System.Type[] { typeof(int) });
                var instance = ci.Invoke(new object[] { items.Count(), }) as IList;
                foreach (var item in items)
                {
                    instance.Add(item);
                }
                return instance;
            }
            else
            {
                var contractName = AttributedModelServices.GetContractName(serviceType);
                return this.container.GetExportedValue<object>(contractName);
            }
        }

        public void Dispose()
        {
            this.container.Dispose();
            this.container = null;
            this.OnDisposed(EventArgs.Empty);
        }

        public event EventHandler Disposed;

        public virtual IEnumerable<Tuple<System.Type, object>> GetParts()
        {
            yield return new Tuple<System.Type, object>(typeof(CremaBootstrapper), this);
            yield return new Tuple<System.Type, object>(typeof(IServiceProvider), this);
        }

        public virtual IEnumerable<Assembly> GetAssemblies()
        {
            var assemblyList = new List<Assembly>();

            if (Assembly.GetEntryAssembly() != null)
            {
                assemblyList.Add(Assembly.GetEntryAssembly());
            }

            var query = from directory in EnumerableUtility.Friends(AppDomain.CurrentDomain.BaseDirectory, this.SelectPath())
                        let catalog = new DirectoryCatalog(directory)
                        from file in catalog.LoadedFiles
                        select file;

            foreach (var item in query)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(item);
                    assemblyList.Add(assembly);
                    CremaLog.Debug(assembly.Location);
                }
                catch
                {

                }
            }

            return assemblyList.Distinct();
        }

        public string Culture
        {
            get
            {
                return $"{System.Globalization.CultureInfo.CurrentCulture}";
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value != string.Empty)
                {
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(value);
                    System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(value);
                }
            }
        }

        protected virtual IEnumerable<string> SelectPath()
        {
            var dllPath = AppDomain.CurrentDomain.BaseDirectory;
            var rootPath = Path.GetDirectoryName(dllPath);
            var pluginsPath = Path.Combine(rootPath, pluginsString);
            if (Directory.Exists(pluginsPath) == true)
            {
                foreach (var item in Directory.GetDirectories(pluginsPath))
                {
                    yield return item;
                }
            }
        }

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        private void Initialize()
        {
            var catalog = new AggregateCatalog();

            foreach (var item in this.GetAssemblies())
            {
                catalog.Catalogs.Add(new AssemblyCatalog(item));
            }

            this.container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            foreach (var item in this.GetParts())
            {
                var contractName = AttributedModelServices.GetContractName(item.Item1);
                var typeIdentity = AttributedModelServices.GetTypeIdentity(item.Item1);
                batch.AddExport(new Export(contractName, new Dictionary<string, object>
                {
                    {
                        "ExportTypeIdentity",
                        typeIdentity
                    }
                }, () => item.Item2));
            }

            this.container.Compose(batch);
        }

        internal static TimeSpan DefaultInactivityTimeout
        {
            get;
            set;
        }
    }
}
