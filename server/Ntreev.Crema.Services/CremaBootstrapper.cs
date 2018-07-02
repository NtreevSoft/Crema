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
using Ntreev.Crema.Data.Xml.Schema;
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
        private CremaSettings settings = new CremaSettings();
        private CompositionContainer container;

        public CremaBootstrapper()
        {
            this.Initialize();
        }

        public static void CreateRepository(IServiceProvider serviceProvider, string basePath, string repositoryModule, string fileType, bool force)
        {
            ValidateCreateRepository(serviceProvider, basePath, repositoryModule, fileType, force);
            var repositoryProvider = GetRepositoryProvider(serviceProvider, repositoryModule);
            var serializer = GetSerializer(serviceProvider, fileType);

            var tempPath = PathUtility.GetTempPath(true);
            var repositoryPath = Path.Combine(basePath, CremaString.Repository);

            DirectoryUtility.Backup(repositoryPath);
            try
            {
                var usersRepo = DirectoryUtility.Prepare(repositoryPath, CremaString.Users);
                var usersPath = DirectoryUtility.Prepare(tempPath, CremaString.Users);
                var dataBasesRepo = DirectoryUtility.Prepare(repositoryPath, CremaString.DataBases);
                var dataBasesPath = DirectoryUtility.Prepare(tempPath, CremaString.DataBases);
                var dataSet = new CremaDataSet();

                UserContext.GenerateDefaultUserInfos(usersPath, serializer);
                repositoryProvider.InitializeRepository(usersRepo, usersPath);

                dataSet.WriteToDirectory(dataBasesPath);
                FileUtility.WriteAllText($"{CremaSchema.MajorVersion}.{CremaSchema.MinorVersion}", dataBasesPath, ".version");
                repositoryProvider.InitializeRepository(dataBasesRepo, dataBasesPath);

                var repoModulePath = FileUtility.WriteAllText(repositoryProvider.Name, repositoryPath, CremaString.Repo);
                var fileTypePath = FileUtility.WriteAllText(serializer.Name, repositoryPath, CremaString.File);

                FileUtility.SetReadOnly(repoModulePath, true);
                FileUtility.SetReadOnly(fileTypePath, true);
                DirectoryUtility.SetVisible(repositoryPath, false);
                DirectoryUtility.Clean(repositoryPath);
            }
            catch
            {
                DirectoryUtility.Restore(repositoryPath);
                throw;
            }
        }

        public static void ValidateRepository(IServiceProvider serviceProvider, string basePath, params string[] dataBaseNames)
        {
            var repositoryModule = FileUtility.ReadAllText(basePath, CremaString.Repository, CremaString.Repo);
            var fileType = FileUtility.ReadAllText(basePath, CremaString.Repository, CremaString.File);
            var repositoryProvider = GetRepositoryProvider(serviceProvider, repositoryModule);
            var serializer = GetSerializer(serviceProvider, fileType);
            var validationPath = Path.Combine(basePath, "validation");
            var logService = new LogServiceHost("validation", validationPath, true) { Verbose = LogVerbose.Info, };

            var repositoryPath = CremaHost.GetPath(basePath, CremaPath.RepositoryDataBases);
            var items = repositoryProvider.GetRepositories(repositoryPath);

            if (dataBaseNames.Length > 0)
                items = items.Intersect(dataBaseNames).ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var tempPath = Path.Combine(validationPath, item);
                var repositorySettings = new RepositorySettings()
                {
                    BasePath = repositoryPath,
                    RepositoryName = item,
                    WorkingPath = tempPath,
                    LogService = logService,
                };
                var repository = repositoryProvider.CreateInstance(repositorySettings);
                try
                {
                    serializer.Validate(repository.BasePath, typeof(CremaDataSet), ObjectSerializerSettings.Empty);
                    repository.Dispose();
                    DirectoryUtility.Delete(tempPath);
                    logService.Info($"[{i}/{items.Length}]{item}: OK");
                }
                catch (Exception e)
                {
                    logService.Error(e);
                    logService.Info($"[{i}/{items.Length}]{item}: Fail");
                }
            }
        }

        public static void UpgradeRepository(IServiceProvider serviceProvider, string basePath, params string[] dataBaseNames)
        {
            var repositoryModule = FileUtility.ReadAllText(basePath, CremaString.Repository, CremaString.Repo);
            var fileType = FileUtility.ReadAllText(basePath, CremaString.Repository, CremaString.File);
            var repositoryProvider = GetRepositoryProvider(serviceProvider, repositoryModule);
            var serializer = GetSerializer(serviceProvider, fileType);
            var upgradePath = Path.Combine(basePath, "upgrade");
            var logService = new LogServiceHost("upgrade", upgradePath, true) { Verbose = LogVerbose.Info, };

            var repositoryPath = CremaHost.GetPath(basePath, CremaPath.RepositoryDataBases);
            var items = repositoryProvider.GetRepositories(repositoryPath);

            if (dataBaseNames.Length > 0)
                items = items.Intersect(dataBaseNames).ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var tempPath = Path.Combine(upgradePath, item);
                var repositorySettings = new RepositorySettings()
                {
                    BasePath = repositoryPath,
                    RepositoryName = item,
                    WorkingPath = tempPath,
                    LogService = logService,
                };
                try
                {
                    if (UpgradeRepository(repositoryProvider, serializer, repositorySettings) == true)
                    {
                        logService.Info($"[{i}/{items.Length}]{item}: upgraded");
                    }
                    else
                    {
                        logService.Info($"[{i}/{items.Length}]{item}: skip");
                    }
                    DirectoryUtility.Delete(tempPath);
                }
                catch (Exception e)
                {
                    logService.Error(e);
                    logService.Info($"[{i}/{items.Length}]{item}: fail");
                }
            }
        }

        public static void MigrateRepository(IServiceProvider serviceProvider, string basePath, string migrationModule, string repositoryUrl, bool force)
        {
            RepositoryMigrator.Migrate(serviceProvider, basePath, migrationModule, repositoryUrl, force);
        }

        public static void CleanRepository(IServiceProvider serviceProvider, string basePath)
        {
            DirectoryUtility.Delete(CremaHost.GetPath(basePath, CremaPath.Caches));
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
                var fullpath = PathUtility.GetFullPath(value);
                this.settings.BasePath = PathUtility.GetCaseSensitivePath(fullpath);
            }
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

        internal static IObjectSerializer GetSerializer(IServiceProvider serviceProvider, string fileType)
        {
            var serializers = serviceProvider.GetService(typeof(IEnumerable<IObjectSerializer>)) as IEnumerable<IObjectSerializer>;
            var serializer = serializers.FirstOrDefault(item => item.Name == (fileType ?? "xml"));
            if (serializer == null)
                throw new InvalidOperationException("no serializer");
            return serializer;
        }

        internal static IRepositoryProvider GetRepositoryProvider(IServiceProvider serviceProvider, string repositoryModule)
        {
            var repositoryProviders = serviceProvider.GetService(typeof(IEnumerable<IRepositoryProvider>)) as IEnumerable<IRepositoryProvider>;
            var repositoryProvider = repositoryProviders.FirstOrDefault(item => item.Name == (repositoryModule ?? "svn"));
            if (repositoryProvider == null)
                throw new InvalidOperationException(Resources.Exception_NoRepositoryModule);
            return repositoryProvider;
        }

        internal static IRepositoryMigrator GetRepositoryMigrator(IServiceProvider serviceProvider, string migrationModule)
        {
            var repositoryUpgraders = serviceProvider.GetService(typeof(IEnumerable<IRepositoryMigrator>)) as IEnumerable<IRepositoryMigrator>;
            var repositoryUpgrader = repositoryUpgraders.FirstOrDefault(item => item.Name == migrationModule);
            if (repositoryUpgrader == null)
                throw new InvalidOperationException(Resources.Exception_NoRepositoryModule);
            return repositoryUpgrader;
        }

        private void Initialize()
        {
            CremaLog.Debug("Initialize.");
            this.OnInitialize();
            CremaLog.Debug("Initialized.");
        }

        private static void ValidateCreateRepository(IServiceProvider serviceProvider, string basePath, string repositoryModule, string fileType, bool force)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (basePath == null)
                throw new ArgumentNullException(nameof(basePath));

            var directoryInfo = new DirectoryInfo(basePath);
            if (directoryInfo.Exists == false && force == true)
            {
                DirectoryUtility.Create(directoryInfo.FullName);
            }
            if (DirectoryUtility.IsEmpty(directoryInfo.FullName) == false && force == false)
            {
                throw new ArgumentException("Path is not an empty directory.", nameof(basePath));
            }
        }

        private static bool UpgradeRepository(IRepositoryProvider repositoryProvider, IObjectSerializer serializer, RepositorySettings settings)
        {
            var repository = repositoryProvider.CreateInstance(settings);
            var versionPath = Path.Combine(repository.BasePath, ".version");
            var version = GetVersion(versionPath);
            if (version.Major == CremaSchema.MajorVersion && version.Minor == CremaSchema.MinorVersion)
            {
                repository.Dispose();
                return false;
            }
            else
            {
                var dataSet = serializer.Deserialize(repository.BasePath, typeof(CremaDataSet), ObjectSerializerSettings.Empty) as CremaDataSet;
                var files = serializer.Serialize(repository.BasePath, dataSet, ObjectSerializerSettings.Empty);

                File.WriteAllText(versionPath, $"{CremaSchema.MajorVersion}.{CremaSchema.MinorVersion}");
                foreach (var item in repository.Status())
                {
                    if (item.Status == RepositoryItemStatus.Untracked)
                    {
                        repository.Add(item.Path);
                    }
                }
                repository.Commit(Authentication.SystemID, $"upgrade: {CremaSchema.MajorVersion}.{CremaSchema.MinorVersion}");
                repository.Dispose();
                return true;
            }
        }

        private static Version GetVersion(string versionPath)
        {
            if (File.Exists(versionPath) == false)
                return new Version();
            try
            {
                return new Version(File.ReadAllText(versionPath));
            }
            catch
            {
                return new Version();
            }
        }
    }
}
