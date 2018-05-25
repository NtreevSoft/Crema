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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.Users;
using Ntreev.Crema.Data.Xml.Schema;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using System.ComponentModel.Composition.Primitives;
using Ntreev.Crema.Data;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services
{
    public class CremaBootstrapper : IServiceProvider, IDisposable
    {
        private const string pluginsString = "plugins";
        private const string repoModulesString = "repo-modules";
        private const string defaultString = "default";
        private const string trunkString = "trunk";
        private const string tagsString = "tags";
        private const string branchesString = "branches";
        private CremaSettings settings = new CremaSettings();
        private CompositionContainer container;

        public CremaBootstrapper()
        {
            this.Initialize();
            this.settings.RepositoryModule = DefaultRepositoryModule;
            CremaLog.Debug("default repository module : {0}", this.settings.RepositoryModule);
        }

        public void CreateRepository(string path, string repositoryName, bool force)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists == false && force == true)
            {
                DirectoryUtility.Create(directoryInfo.FullName);
            }

            if (DirectoryUtility.IsEmpty(directoryInfo.FullName) == false && force == false)
            {
                throw new ArgumentException(Resources.Exception_PathIsNotEmptyDirectory, nameof(path));
            }

            try
            {
                var basePath = directoryInfo.FullName;
                var repoProviders = this.GetInstances(typeof(IRepositoryProvider)).OfType<IRepositoryProvider>();
                var repoProvider = repoProviders.FirstOrDefault(item => item.Name == this.RepositoryModule);
                if (repoProvider == null)
                    throw new InvalidOperationException(Resources.Exception_NoRepositoryModule);
                //var repositoryPath = Path.Combine(basePath, repositoryName);
                var tempPath = PathUtility.GetTempPath(true);
                ////DirectoryUtility.Create(repositoryPath);
                ////DirectoryUtility.Prepare(repositoryPath, tagsString);
                ////DirectoryUtility.Prepare(repositoryPath, branchesString);
                ////DirectoryUtility.Prepare(repositoryPath, trunkString);
                ////DirectoryUtility.Prepare(repositoryPath, trunkString, CremaSchema.TypeDirectory);
                ////DirectoryUtility.Prepare(repositoryPath, trunkString, CremaSchema.TableDirectory);

                ////var dataBasesPath = DirectoryUtility.Prepare(tempPath, "databases");
                ////var defaultPath = DirectoryUtility.Prepare(dataBasesPath, "default");
                var usersRepo = DirectoryUtility.Prepare(basePath, "users");
                var usersPath = DirectoryUtility.Prepare(tempPath, "users");
                 UserContext.GenerateDefaultUserInfos(usersPath);
                repoProvider.InitializeRepository(usersRepo, usersPath);


                var dataBasesRepo = DirectoryUtility.Prepare(basePath, "databases");
                var dataBasesPath = DirectoryUtility.Prepare(tempPath, "databases");
                new CremaDataSet().WriteToDirectory(dataBasesPath);
                repoProvider.InitializeRepository(dataBasesRepo, dataBasesPath);
            }
            catch
            {
                throw;
            }
        }

        public void ValidateRepository(string path, string repositoryName)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists == false)
                throw new DirectoryNotFoundException();

            var basePath = directoryInfo.FullName;
            var repoProviders = this.GetInstances(typeof(IRepositoryProvider)).OfType<IRepositoryProvider>();
            var repoProvider = repoProviders.FirstOrDefault(item => item.Name == this.RepositoryModule);
            var repositoryPath = Path.Combine(basePath, repositoryName);
            var trunkPath = Path.Combine(repositoryPath, trunkString);
            var tagsPath = Path.Combine(repositoryPath, tagsString);
            var branchesPath = Path.Combine(repositoryPath, branchesString);

            CremaDataSet.ValidateDirectory(trunkPath);
            foreach (var item in Directory.GetDirectories(tagsPath))
            {
                CremaDataSet.ValidateDirectory(item);
            }
            foreach (var item in Directory.GetDirectories(branchesPath))
            {
                CremaDataSet.ValidateDirectory(item);
            }
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;

            if (typeof(IEnumerable).IsAssignableFrom(serviceType) && serviceType.GenericTypeArguments.Length == 1)
            {
                var itemType = serviceType.GenericTypeArguments.First();
                var items = this.GetInstances(itemType);
                var listGenericType = typeof(List<>);
                var list = listGenericType.MakeGenericType(itemType);
                var ci = list.GetConstructor(new System.Type[] { typeof(int) });
                var instance = ci.Invoke(new object[] { items.Count() }) as IList;
                foreach (var item in items)
                {
                    instance.Add(item);
                }
                return instance;
            }
            else
            {
                return this.GetInstance(serviceType);
            }
        }

        public void Dispose()
        {
            this.container?.Dispose();
            this.container = null;
            this.OnDisposed(EventArgs.Empty);
        }

        public virtual IEnumerable<Tuple<System.Type, object>> GetParts()
        {
            yield return new Tuple<System.Type, object>(typeof(CremaBootstrapper), this);
            yield return new Tuple<System.Type, object>(typeof(IServiceProvider), this);
            yield return new Tuple<System.Type, object>(typeof(CremaSettings), this.settings);
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

        public virtual IEnumerable<string> SelectPath()
        {
            var dllPath = AppDomain.CurrentDomain.BaseDirectory;
            var rootPath = Path.GetDirectoryName(dllPath);
            var repositoryPath = Path.Combine(rootPath, RepositoryModulesPath);
            if (Directory.Exists(repositoryPath) == true)
            {
                foreach (var item in Directory.GetDirectories(repositoryPath))
                {
                    yield return item;
                }
            }

            var pluginsPath = Path.Combine(rootPath, pluginsString);
            if (Directory.Exists(pluginsPath) == true)
            {
                foreach (var item in Directory.GetDirectories(pluginsPath))
                {
                    yield return item;
                }
            }
        }

        public string BasePath
        {
            get { return this.settings.BasePath; }
            set
            {
                var fullpath = Path.GetFullPath(value);
                this.settings.BasePath = PathUtility.GetCaseSensitivePath(fullpath);
            }
        }

        public string RepositoryName
        {
            get { return this.settings.RepositoryName; }
            set { this.settings.RepositoryName = value; }
        }

        public bool MultiThreading
        {
            get { return this.settings.MultiThreading; }
            set { this.settings.MultiThreading = value; }
        }

        public LogVerbose Verbose
        {
            get { return this.settings.Verbose; }
            set { this.settings.Verbose = value; }
        }

        public string RepositoryModule
        {
            get { return this.settings.RepositoryModule; }
            set { this.settings.RepositoryModule = value ?? DefaultRepositoryModule; }
        }

        public bool NoCache
        {
            get { return this.settings.NoCache; }
            set { this.settings.NoCache = value; }
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

        public string[] DataBaseList
        {
            get { return this.settings.DataBaseList; }
            set { this.settings.DataBaseList = value; }
        }

#if DEBUG

        public bool ValidationMode
        {
            get { return this.settings.ValidationMode; }
            set { this.settings.ValidationMode = value; }
        }

#endif

        public static string DefaultRepositoryModule
        {
            get { return "svn"; }
        }

        public static string RepositoryModulesPath
        {
            get { return repoModulesString; }
        }

        public event EventHandler Disposed;

        protected virtual object GetInstance(System.Type type)
        {
            var contractName = AttributedModelServices.GetContractName(type);
            return this.container.GetExportedValue<object>(contractName);
        }

        protected virtual IEnumerable<object> GetInstances(System.Type type)
        {
            var contractName = AttributedModelServices.GetContractName(type);
            return this.container.GetExportedValues<object>(contractName);
        }

        protected virtual void OnInitialize()
        {
            var catalog = new AggregateCatalog();

            foreach (var item in this.GetAssemblies())
            {
                catalog.Catalogs.Add(new AssemblyCatalog(item));
            }

            this.container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddPart(this.settings);
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

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        private void Initialize()
        {
            CremaLog.Debug("Initialize.");
            this.OnInitialize();
            CremaLog.Debug("Initialized.");
        }
    }
}
