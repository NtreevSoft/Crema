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

using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Security;
using System.Timers;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Services
{
    [Export(typeof(ILogService))]
    [InheritedExport(typeof(ICremaHost))]
    class CremaHost : ICremaHost, ILogService, IDisposable
    {
        private const string workingString = "working";
        private const string trunkString = "trunk";
        private const string tagsString = "tags";
        private const string branchesString = "branches";

        private readonly string basePath;
        private readonly string repositoryPath;
        private readonly string trunkPath;
        private readonly string tagsPath;
        private readonly string branchesPath;
        private readonly string workingPath;

        private CremaConfiguration configs;
        private IEnumerable<IPlugin> plugins;

        [Import]
        private IServiceProvider container = null;
        private CremaSettings settings;
        private IRepositoryProvider repositoryProvider;
        private IRepository repository;
        private readonly LogService log;
        private DomainContext domainContext;
        private UserContext userContext;
        private DataBaseCollection dataBases;
        private CremaDispatcher dispatcher;
        private CremaDispatcher repositoryDispatcher;
        private Guid token;
        private ShutdownTimer shutdownTimer;

        [ImportMany]
        private IEnumerable<IConfigurationPropertyProvider> propertiesProvider = null;

        [ImportingConstructor]
        public CremaHost(CremaSettings settings, [ImportMany]IEnumerable<IRepositoryProvider> repoProviders)
        {
            CremaLog.Debug("crema instance created.");
            this.settings = settings;
            this.basePath = settings.BasePath;
            this.repositoryPath = Path.Combine(settings.BasePath, settings.RepositoryName);
            this.trunkPath = Path.Combine(this.repositoryPath, trunkString);
            this.tagsPath = Path.Combine(this.repositoryPath, tagsString);
            this.branchesPath = Path.Combine(this.repositoryPath, branchesString);
            this.workingPath = Path.Combine(this.basePath, workingString);
            this.repositoryProvider = repoProviders.First(item => item.Name == this.settings.RepositoryModule);
            this.repositoryProvider.ValidateRepository(this.basePath, this.repositoryPath);

            this.log = new LogService(this.GetType().FullName, this.WorkingPath)
            {
                Name = "repository",
                Verbose = settings.Verbose
            };
            CremaLog.Debug("crema log service initialized.");
            CremaLog.Debug($"available tags : {string.Join(", ", TagInfoUtility.Names)}");
            if (settings.MultiThreading == true)
                this.dispatcher = new CremaDispatcher(this);
            else
                this.dispatcher = new CremaDispatcher(this, System.Windows.Threading.Dispatcher.CurrentDispatcher);
            this.repositoryDispatcher = new CremaDispatcher(this.repositoryProvider);
            CremaLog.Debug("crema dispatcher initialized.");
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == typeof(ICremaHost))
                return this;
            if (serviceType == typeof(IDataBaseCollection))
                return this.dataBases;
            if (serviceType == typeof(IUserContext))
                return this.userContext;
            if (serviceType == typeof(IUserCollection))
                return this.userContext.Users;
            if (serviceType == typeof(IUserCategoryCollection))
                return this.userContext.Categories;
            if (serviceType == typeof(IDomainContext))
                return this.domainContext;
            if (serviceType == typeof(IDomainCollection))
                return this.domainContext.Domains;
            if (serviceType == typeof(IDomainCategoryCollection))
                return this.domainContext.Categories;
            if (serviceType == typeof(ILogService))
                return this;
            if (this.IsOpened == true && serviceType == typeof(ICremaConfiguration))
                return this.configs;

            if (this.container != null)
            {
                try
                {
                    return this.container.GetService(serviceType);
                }
                catch (Exception e)
                {
                    CremaLog.Error(e);
                    return null;
                }
            }

            return null;
        }

        public void Open()
        {
            this.OnOpening(EventArgs.Empty);
            this.dispatcher.Invoke(() =>
            {
                this.Info(Resources.Message_ProgramInfo, AppUtility.ProductName, AppUtility.ProductVersion);

                this.repository = this.repositoryProvider.CreateInstance(this.repositoryPath, this.workingPath);
                this.Info("Repository module : {0}", this.settings.RepositoryModule);
                this.Info(Resources.Message_ServiceStart);

                this.configs = new CremaConfiguration(Path.Combine(this.basePath, "configs.xml"), this.propertiesProvider);

                this.userContext = new UserContext(this);
                this.userContext.Dispatcher.Invoke(() => this.userContext.Initialize());
                this.dataBases = new DataBaseCollection(this);
                this.domainContext = new DomainContext(this, this.userContext);

                if (this.settings.NoCache == false)
                {
                    foreach (var item in this.dataBases)
                    {
                        this.domainContext.Restore(Authentication.System, item);
                    }
                }

                this.plugins = this.container.GetService(typeof(IEnumerable<IPlugin>)) as IEnumerable<IPlugin>;
                this.plugins = this.plugins.TopologicalSort();
                foreach (var item in this.plugins)
                {
                    var authentication = new Authentication(new AuthenticationProvider(item), item.ID);
                    item.Initialize(authentication);
                    this.Info("Plugin : {0}", item.Name);
                }

                this.dispatcher.InvokeAsync(() => this.dataBases.RestoreState(this.settings));

                GC.Collect();
                this.Info("Crema module has been started.");
                this.OnOpened(EventArgs.Empty);
            });
        }

        public void SaveConfigs()
        {
            this.configs.Commit();
        }

        public void Close(CloseReason reason, string message)
        {
            this.OnClosing(EventArgs.Empty);
            this.dispatcher.Invoke(() =>
            {
                this.userContext.Dispatcher.Invoke(() => this.userContext.Clear());
                this.UserContext.Dispose();
                this.userContext = null;
                this.domainContext.Clear();
                this.domainContext.Dispose();
                this.domainContext = null;
                this.dataBases.Dispose();
                this.dataBases = null;
                foreach (var item in this.plugins.Reverse())
                {
                    item.Release();
                }
                this.Info("Crema module has been stopped.");
                this.OnClosed(new ClosedEventArgs(reason, message));
            });
        }

        public void Shutdown(Authentication authentication, int milliseconds, ShutdownType shutdownType, string message)
        {
            this.DebugMethod(authentication, this, nameof(Shutdown), this, milliseconds, shutdownType, message);
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (milliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(milliseconds), "invalid milliseconds value");

            if (string.IsNullOrEmpty(message) == false)
                this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, message));

            if (this.shutdownTimer == null)
            {
                this.shutdownTimer = new ShutdownTimer()
                {
                    Interval = 1000,
                };
                this.shutdownTimer.Elapsed += ShutdownTimer_Elapsed;
            }

            var dateTime = DateTime.Now.AddMilliseconds(milliseconds);
            this.shutdownTimer.DateTime = dateTime;
            this.shutdownTimer.ShutdownType = shutdownType;
            this.shutdownTimer.Start();
            if (milliseconds >= 1000)
                this.SendShutdownMessage((dateTime - DateTime.Now) + new TimeSpan(0, 0, 0, 0, 500), true);
        }

        public void CancelShutdown(Authentication authentication)
        {
            this.DebugMethod(authentication, this, nameof(CancelShutdown), this);
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            if (this.shutdownTimer != null)
            {
                this.shutdownTimer.Elapsed -= ShutdownTimer_Elapsed;
                this.shutdownTimer.Stop();
                this.shutdownTimer.Dispose();
                this.shutdownTimer = null;
                this.Info($"[{authentication}] Shutdown cancelled.");
            }
        }

        public Authentication Login(string userID, SecureString password)
        {
            return this.userContext.Dispatcher.Invoke(() => this.userContext.Login(userID, password));
        }

        public void Logout(Authentication authentication)
        {
            this.userContext.Dispatcher.Invoke(() => this.userContext.Logout(authentication));
        }

        public void Debug(object message)
        {
            this.log.Debug(message);
        }

        public void Info(object message)
        {
            this.log.Info(message);
        }

        public void Error(object message)
        {
            this.log.Error(message);
        }

        public void Warn(object message)
        {
            this.log.Warn(message);
        }

        public void Fatal(object message)
        {
            this.log.Fatal(message);
        }

        public void Dispose()
        {
            if (this.dataBases != null)
            {
                throw new InvalidOperationException(Resources.Exception_NotClosed);
            }
            this.repositoryDispatcher.Dispose();
            this.repositoryDispatcher = null;
            this.dispatcher.Dispose();
            this.dispatcher = null;
            this.OnDisposed(EventArgs.Empty);
            CremaLog.Release();
        }

        public IRepository Repository
        {
            get { return this.repository; }
        }

        public string BasePath
        {
            get { return this.basePath; }
        }

        public string RepositoryPath
        {
            get { return this.repositoryPath; }
        }

        public string WorkingPath
        {
            get { return this.workingPath; }
        }

        public string TrunkPath
        {
            get { return this.trunkPath; }
        }

        public string TagsPath
        {
            get { return this.tagsPath; }
        }

        public string BranchesPath
        {
            get { return this.branchesPath; }
        }

        public bool IsOpened
        {
            get { return this.dataBases != null; }
        }

        public bool NoCache
        {
            get { return this.settings.NoCache; }
        }

        public LogVerbose Verbose
        {
            get { return this.log.Verbose; }
            set { this.log.Verbose = value; }
        }

        public DataBaseCollection DataBases
        {
            get { return this.dataBases; }
        }

        public DomainContext DomainContext
        {
            get { return this.domainContext; }
        }

        public UserContext UserContext
        {
            get { return this.userContext; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        public CremaDispatcher RepositoryDispatcher
        {
            get { return this.repositoryDispatcher; }
        }

        public event EventHandler Opening;

        public event EventHandler Opened;

        public event EventHandler Closing;

        public event ClosedEventHandler Closed;

        public event EventHandler Disposed;

        protected virtual void OnOpening(EventArgs e)
        {
            this.Opening?.Invoke(this, e);
        }

        protected virtual void OnOpened(EventArgs e)
        {
            this.Opened?.Invoke(this, e);
        }

        protected virtual void OnClosing(EventArgs e)
        {
            this.Closing?.Invoke(this, e);
        }

        protected virtual void OnClosed(ClosedEventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        private void ShutdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now >= this.shutdownTimer.DateTime)
            {
                this.Shutdown(this.shutdownTimer.ShutdownType);
            }
            else
            {
                var timeSpan = this.shutdownTimer.DateTime - DateTime.Now;
                this.SendShutdownMessage(timeSpan, false);
            }
        }

        private void SendShutdownMessage(TimeSpan timeSpan, bool about)
        {
            if (about == true)
            {
                if (timeSpan.TotalSeconds >= 3600)
                {
                    this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, $"crema shuts down after about {timeSpan.Hours} hours."));
                }
                else if (timeSpan.TotalSeconds >= 60)
                {
                    this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, $"crema shuts down after about {timeSpan.Minutes} minutes."));
                }
                else
                {
                    this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, $"crema shuts down after about {timeSpan.Seconds} seconds."));
                }
            }
            else
            {
                if (timeSpan.TotalSeconds % 3600 == 0)
                {
                    this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, $"crema shuts down after {timeSpan.Hours} hours."));
                }
                else if (timeSpan.TotalSeconds % 60 == 0)
                {
                    this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, $"crema shuts down after {timeSpan.Minutes} minutes."));
                }
                else if (timeSpan.TotalSeconds == 30 || timeSpan.TotalSeconds == 15 || timeSpan.TotalSeconds == 10 || timeSpan.TotalSeconds <= 5)
                {
                    this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(Authentication.System, $"crema shuts down after {timeSpan.Seconds} seconds."));
                }
            }
        }

        private void Shutdown(ShutdownType shutdownType)
        {
            if (this.shutdownTimer != null)
            {
                this.shutdownTimer.Elapsed -= ShutdownTimer_Elapsed;
                this.shutdownTimer.Stop();
                this.shutdownTimer.Dispose();
                this.shutdownTimer = null;
            }

            var isRestart = shutdownType.HasFlag(ShutdownType.Restart);
            this.Close(isRestart ? CloseReason.Restart : CloseReason.Shutdown, string.Empty);
            if (isRestart == true)
            {
                this.settings.NoCache = shutdownType.HasFlag(ShutdownType.NoCache);
                this.Open();
            }
        }

        #region classes

        class ShutdownTimer : Timer
        {
            public DateTime DateTime
            {
                get; set;
            }

            public ShutdownType ShutdownType
            {
                get; set;
            }
        }

        #endregion

        #region ILogService

        LogVerbose ILogService.Verbose
        {
            get { return this.log.Verbose; }
            set { this.log.Verbose = value; }
        }

        TextWriter ILogService.RedirectionWriter
        {
            get { return this.log.RedirectionWriter; }
            set { this.log.RedirectionWriter = value; }
        }

        string ILogService.Name
        {
            get { return this.log.Name; }
        }

        string ILogService.FileName
        {
            get { return this.log.FileName; }
        }

        bool ILogService.IsEnabled
        {
            get { return this.IsOpened; }
        }

        #endregion

        #region ICremaHost

        Guid ICremaHost.Open()
        {
            this.Open();
            this.token = Guid.NewGuid();
            return this.token;
        }

        void ICremaHost.Close(Guid token)
        {
            if (this.token != token)
                throw new InvalidOperationException(Resources.Exception_InvalidToken);
            this.Close(CloseReason.None, string.Empty);
            this.token = Guid.Empty;
        }

        IDataBaseCollection ICremaHost.DataBases
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.DataBases;
            }
        }

        ICremaConfiguration ICremaHost.Configs
        {
            get { return this.configs; }
        }

        #endregion
    }
}
