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

using Ntreev.Crema.Data;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ntreev.Crema.Services
{
    public class CremaBootstrapper : IServiceProvider, IDisposable
    {
        private const string pluginsString = "plugins";
        private const string serializersString = "serializers";
        private const string repoModulesString = "repo-modules";
        private const string defaultString = "default";
        private CremaSettings settings = new CremaSettings();
        private CompositionContainer container;

        public CremaBootstrapper()
        {
            this.Initialize();
            CremaLog.Debug("default repository module : {0}", this.settings.RepositoryModule);
        }

        public static void CreateRepository(IServiceProvider serviceProvider, RepositoryCreationSettings settings)
        {
            var directoryInfo = new DirectoryInfo(settings.BasePath);

            if (directoryInfo.Exists == false && settings.Force == true)
            {
                DirectoryUtility.Create(directoryInfo.FullName);
            }

            if (DirectoryUtility.IsEmpty(directoryInfo.FullName) == false && settings.Force == false)
            {
                throw new ArgumentException(Resources.Exception_PathIsNotEmptyDirectory, nameof(settings.BasePath));
            }

            try
            {
                var basePath = directoryInfo.FullName;
                var repositoryProvider = GetRepositoryProvider(serviceProvider, settings.RepositoryModule);
                var serializer = GetSerializer(serviceProvider, settings.FileType);

                var tempPath = PathUtility.GetTempPath(true);
                var usersRepo = DirectoryUtility.Prepare(basePath, "remotes", "users");
                var usersPath = DirectoryUtility.Prepare(tempPath, "remotes", "users");

                UserContext.GenerateDefaultUserInfos(usersPath, serializer);
                repositoryProvider.InitializeRepository(usersRepo, usersPath);

                var dataBasesRepo = DirectoryUtility.Prepare(basePath, "remotes", "databases");
                var dataBasesPath = DirectoryUtility.Prepare(tempPath, "remotes", "databases");
                new CremaDataSet().WriteToDirectory(dataBasesPath);
                repositoryProvider.InitializeRepository(dataBasesRepo, dataBasesPath);
            }
            catch
            {
                throw;
            }
        }

        public static void ValidateRepository(IServiceProvider serviceProvider, RepositoryValidationSettings settings)
        {
            var repositoryProvider = GetRepositoryProvider(serviceProvider, settings.RepositoryModule);
            var serializer = GetSerializer(serviceProvider, settings.FileType);

            var logService = new LogServiceHost(serviceProvider.GetType().FullName, CremaHost.GetPath(settings.GetTempPath(), CremaPath.Logs))
            {
                Verbose = settings.Verbose
            };

            var dataBasesPath = Path.Combine(settings.BasePath, "remotes", "databases");
            var items = repositoryProvider.GetRepositories(dataBasesPath);

            if (settings.DataBaseNames.Length > 0)
                items = items.Intersect(settings.DataBaseNames).ToArray();

            foreach (var item in items)
            {
                var tempPath = settings.GetTempPath(item);
                var repositorySettings = new RepositorySettings()
                {
                    BasePath = dataBasesPath,
                    RepositoryName = item,
                    WorkingPath = tempPath,
                    LogService = logService,
                };
                var repository = repositoryProvider.CreateInstance(repositorySettings);
                try
                {
                    serializer.Validate(repository.BasePath, typeof(CremaDataSet), ObjectSerializerSettings.Empty);
                }
                catch (Exception e)
                {
                    logService.Error(e);
                    throw;
                }
                repository.Dispose();
                DirectoryUtility.Delete(tempPath);
            }
            logService.Info("end");
        }

        public static void MigrateRepository(IServiceProvider serviceProvider, RepositoryMigrationSettings settings)
        {
            var repositoryProvider = GetRepositoryProvider(serviceProvider, settings.RepositoryModule);
            var serializer = GetSerializer(serviceProvider, settings.FileType);

            var logService = new LogServiceHost(serviceProvider.GetType().FullName, CremaHost.GetPath(settings.BasePath, CremaPath.Logs))
            {
                Verbose = settings.Verbose
            };

            var basePath = Path.Combine(settings.BasePath, "remotes", "databases");
            var items = repositoryProvider.GetRepositories(basePath);

            if (settings.DataBaseNames.Length > 0)
                items = items.Intersect(settings.DataBaseNames).ToArray();

            foreach (var item in items)
            {
                var tempPath = settings.GetTempPath(item);
                try
                {
                    var repositorySettings = new RepositorySettings()
                    {
                        BasePath = basePath,
                        RepositoryName = item,
                        WorkingPath = tempPath,
                        LogService = logService,
                    };
                    MigrateRepository(repositoryProvider, serializer, repositorySettings);
                    CremaLog.Info("migtrated : {0}", item);
                }
                finally
                {
                    DirectoryUtility.Delete(tempPath);
                }
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

            var serializersPath = Path.Combine(rootPath, serializersString);
            if (Directory.Exists(serializersPath) == true)
            {
                foreach (var item in Directory.GetDirectories(serializersPath))
                {
                    yield return item;
                }
            }
        }

        public string BasePath
        {
            get => this.settings.BasePath;
            set
            {
                var fullpath = Path.GetFullPath(value);
                this.settings.BasePath = PathUtility.GetCaseSensitivePath(fullpath);
            }
        }

        public string FileType
        {
            get => this.settings.FileType;
            set => this.settings.FileType = value;
        }

        public bool MultiThreading
        {
            get => this.settings.MultiThreading;
            set => this.settings.MultiThreading = value;
        }

        public LogVerbose Verbose
        {
            get => this.settings.Verbose;
            set => this.settings.Verbose = value;
        }

        public string RepositoryModule
        {
            get => this.settings.RepositoryModule;
            set => this.settings.RepositoryModule = value;
        }

        public bool NoCache
        {
            get => this.settings.NoCache;
            set => this.settings.NoCache = value;
        }

        public string Culture
        {
            get => $"{System.Globalization.CultureInfo.CurrentCulture}";
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
            get => this.settings.DataBaseList;
            set => this.settings.DataBaseList = value;
        }

#if DEBUG
        public bool ValidationMode
        {
            get => this.settings.ValidationMode;
            set => this.settings.ValidationMode = value;
        }
#endif

        public static string RepositoryModulesPath => repoModulesString;

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

        private static IObjectSerializer GetSerializer(IServiceProvider serviceProvider, string fileType)
        {
            var serializers = serviceProvider.GetService(typeof(IEnumerable<IObjectSerializer>)) as IEnumerable<IObjectSerializer>;
            var serializer = serializers.FirstOrDefault(item => item.Name == fileType);
            if (serializer == null)
                throw new InvalidOperationException("no serializer");
            return serializer;
        }

        private static IRepositoryProvider GetRepositoryProvider(IServiceProvider serviceProvider, string repositoryModule)
        {
            var repositoryProviders = serviceProvider.GetService(typeof(IEnumerable<IRepositoryProvider>)) as IEnumerable<IRepositoryProvider>;
            var repositoryProvider = repositoryProviders.FirstOrDefault(item => item.Name == repositoryModule);
            if (repositoryProvider == null)
                throw new InvalidOperationException(Resources.Exception_NoRepositoryModule);
            return repositoryProvider;
        }

        private void Initialize()
        {
            CremaLog.Debug("Initialize.");
            this.OnInitialize();
            CremaLog.Debug("Initialized.");
        }

        private static void MigrateRepository(IRepositoryProvider repositoryProvider, IObjectSerializer serializer, RepositorySettings settings)
        {
            using (var repository = repositoryProvider.CreateInstance(settings))
            {
                var dataSet = serializer.Deserialize(repository.BasePath, typeof(CremaDataSet), ObjectSerializerSettings.Empty) as CremaDataSet;
                var files = serializer.Serialize(repository.BasePath, dataSet, ObjectSerializerSettings.Empty);

                var items = repository.Status();
                foreach (var item in items)
                {
                    if (item.Status == RepositoryItemStatus.Untracked)
                    {
                        repository.Add(item.Path);
                    }
                }
                repository.Commit("migration");
            }
        }
    }
}
