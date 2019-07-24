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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Threading;
using System.Xml;

namespace Ntreev.Crema.Services
{
    [Export(typeof(ILogService))]
    [InheritedExport(typeof(ICremaHost))]
    class CremaHost : ICremaHost, IServiceProvider, ILogService
    {
        private readonly List<Authentication> authentications = new List<Authentication>();
        private readonly List<ICremaService> services = new List<ICremaService>();
        private bool isConnected;
        private string userID;
        private Authority authority;
        private string address;
        private CloseInfo closeInfo;

        private CremaConfiguration configs;
        private IEnumerable<IPlugin> plugins;

        [Import]
        private IServiceProvider container = null;
        private LogService log;
        private DomainContext domainContext;
        private UserContext userContext;
        private DataBaseCollection dataBases;
        private CremaDispatcher dispatcher;

        private Dictionary<string, ServiceInfo> serviceInfos;
        private string ipAddress;
        private Guid token;
        private LogVerbose verbose;

        [ImportingConstructor]
        public CremaHost()
        {
            this.dispatcher = new CremaDispatcher(this);
            CremaLog.Debug($"available tags : {string.Join(",", TagInfoUtility.Names)}");
            CremaLog.Debug("Crema created.");
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

        public void AddService(ICremaService service)
        {
            this.services.Add(service);
            CremaLog.Debug($"{service.GetType().Name} Initialized.");
        }

        public async void RemoveService(ICremaService service)
        {
            if (this.services.Contains(service) == false)
                return;
            this.services.Remove(service);
            CremaLog.Debug($"{service.GetType().Name} Released.");
            if (this.services.Any() == false)
            {
                await this.dispatcher.InvokeAsync(() =>
                {
                    this.InvokeClose(this.closeInfo);
                });
            }
        }

        public void RemoveService(ICremaService service, CloseInfo closeInfo)
        {
            this.closeInfo = closeInfo;
            this.RemoveService(service);
        }

        public void InvokeClose(CloseInfo closeInfo)
        {
            this.Close(closeInfo);
        }

        public Guid Open(string address, string userID, SecureString password)
        {
            if (this.isConnected == true)
                throw new InvalidOperationException(Resources.Exception_AlreadyConnected);

            this.ipAddress = AddressUtility.GetIPAddress(address);
            this.serviceInfos = GetServiceInfo(address).ToDictionary(item => item.Name);

            this.OnOpening(EventArgs.Empty);
            return this.dispatcher.Invoke(() =>
            {
                try
                {
                    return OpenInternal(address, null, userID, password);
                }
                catch
                {
                    this.userContext?.Close(CloseInfo.Empty);
                    this.userContext = null;
                    this.log?.Dispose();
                    this.log = null;
                    this.address = null;
                    throw;
                }
            });
        }

        public Guid Open(string address, string dataBase, string userID, SecureString password)
        {
            if (this.isConnected == true)
                throw new InvalidOperationException(Resources.Exception_AlreadyConnected);

            this.ipAddress = AddressUtility.GetIPAddress(address);
            this.serviceInfos = GetServiceInfo(address).ToDictionary(item => item.Name);

            this.OnOpening(EventArgs.Empty);
            return this.dispatcher.Invoke(() =>
            {
                try
                {
                    return OpenInternal(address, dataBase, userID, password);
                }
                catch
                {
                    this.userContext?.Close(CloseInfo.Empty);
                    this.userContext = null;
                    this.log?.Dispose();
                    this.log = null;
                    this.address = null;
                    throw;
                }
            });
        }

        private Guid OpenInternal(string address, string dataBase, string userID, SecureString password)
        {
            this.address = AddressUtility.GetDisplayAddress(address);
            this.log = new LogService(this.address.Replace(':', '_'), userID, AppUtility.UserAppDataPath);
            this.log.Verbose = this.verbose;
            this.userContext = new UserContext(this, this.ipAddress, serviceInfos[nameof(UserService)], userID, password);
            var user = this.userContext.Users[userID];
            user.SetUserState(UserState.Online);
            this.userID = userID;
            this.authority = user.Authority;

            this.dataBases = new DataBaseCollection(this, this.ipAddress, dataBase, serviceInfos[nameof(DataBaseCollectionService)]);
            this.domainContext = new DomainContext(this, this.ipAddress, serviceInfos[nameof(DomainService)]);
            this.isConnected = true;
            this.configs = new CremaConfiguration(this.ConfigPath);
            this.plugins = this.container.GetService(typeof(IEnumerable<IPlugin>)) as IEnumerable<IPlugin>;
            foreach (var item in this.plugins)
            {
                var authentication = new Authentication(new AuthenticationProvider(user), UserContext.AuthenticationToken);
                this.authentications.Add(authentication);
                item.Initialize(authentication);
            }

            this.OnOpened(EventArgs.Empty);
            this.token = UserContext.AuthenticationToken;
            this.token = Guid.NewGuid();
            CremaLog.Info($"Crema opened : {address} {userID}");
            return token;
        }

        public void SaveConfigs()
        {
            this.configs.Commit();
        }

        public void Close(Guid token)
        {
            if (this.token != token)
                throw new ArgumentException(Resources.Exception_InvalidToken, nameof(token));
            if (this.isConnected == false)
                throw new InvalidOperationException(Resources.Exception_NotConnected);

            this.dispatcher.Invoke(() =>
            {
                this.Close(CloseInfo.Empty);
                this.token = Guid.Empty;
            });
        }

        public void Shutdown(Authentication authentication, int milliseconds, ShutdownType shutdownType, string message)
        {
            this.DebugMethod(authentication, this, nameof(Shutdown), this, milliseconds, shutdownType, message);
            var result = this.userContext.Service.Shutdown(milliseconds, shutdownType, message);
            result.Validate();
        }

        public void CancelShutdown(Authentication authentication)
        {
            this.DebugMethod(authentication, this, nameof(CancelShutdown));
            var result = this.userContext.Service.CancelShutdown();
            result.Validate();
        }

        public void Dispose()
        {
            this.ValidateDispose();

            if (Environment.ExitCode != 0 && this.IsOpened == true)
            {
                this.Close(CloseInfo.Empty);
            }

            this.dispatcher.Dispose(false);
            this.dispatcher = null;
            this.OnDisposed(EventArgs.Empty);
            CremaLog.Info("Crema disposed.");
        }

        public void Debug(object message)
        {
            this.Log.Debug(message);
        }

        public void Info(object message)
        {
            this.Log.Info(message);
        }

        public void Error(object message)
        {
            this.Log.Error(message);
        }

        public void Warn(object message)
        {
            this.Log.Warn(message);
        }

        public void Fatal(object message)
        {
            this.Log.Fatal(message);
        }

        public static Binding CreateBinding(ServiceInfo serviceInfo)
        {
            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas = XmlDictionaryReaderQuotas.Max;

            var isDebug = false;
            if (serviceInfo.PlatformID != string.Empty && Enum.TryParse<PlatformID>(serviceInfo.PlatformID, out var platformID) == false)
            {
                isDebug = true;
                platformID = (PlatformID)Enum.Parse(typeof(PlatformID), serviceInfo.PlatformID.Replace("DEBUG_", string.Empty));
            }

#if DEBUG
            if (isDebug == true)
            {
                binding.SendTimeout = TimeSpan.MaxValue;
            }
#endif

            return binding;
        }

        public ConfigurationBase Configs
        {
            get { return this.configs; }
        }

        public string Address
        {
            get { return this.address; }
        }

        public string UserID
        {
            get { return this.userID; }
        }

        public Guid Token => this.token;

        public Authority Authority
        {
            get { return this.authority; }
        }

        public User User
        {
            get
            {
                if (this.IsOpened == false)
                    return null;
                return this.userContext.Users[this.userID];
            }
        }

        public bool IsOpened
        {
            get { return this.isConnected; }
        }

        public LogVerbose Verbose
        {
            get
            {
                if (this.log != null)
                    return this.log.Verbose;
                return this.verbose;
            }
            set
            {
                this.verbose = value;
                if (this.log != null)
                    this.log.Verbose = value;
            }
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

        public Dictionary<string, ServiceInfo> ServiceInfos
        {
            get { return this.serviceInfos; }
        }

        public string IPAddress
        {
            get { return this.ipAddress; }
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

        private void ValidateDispose()
        {
            if (Environment.ExitCode == 0 && this.IsOpened == true)
                throw new InvalidOperationException(Resources.Exception_NotClosed);
            if (this.dispatcher == null)
                throw new InvalidOperationException(Resources.Exception_AlreadyDisposed);
        }

        private void Close(CloseInfo closeInfo)
        {
            this.OnClosing(EventArgs.Empty);
            foreach (var item in this.services.Reverse<ICremaService>())
            {
                item.Close(closeInfo);
            }
            this.services.Clear();
            this.domainContext = null;
            this.dataBases = null;
            this.userContext = null;
            foreach (var item in this.plugins)
            {
                item.Release();
            }
            foreach (var item in this.authentications)
            {
                item.InvokeExpiredEvent(Authentication.SystemID);
            }
            this.dispatcher.InvokeAsync(() =>
            {
                this.log?.Dispose();
                this.log = null;
            });
            this.address = null;
            this.userID = null;
            this.isConnected = false;
            this.OnClosed(new ClosedEventArgs(closeInfo.Reason, closeInfo.Message));
            CremaLog.Info("Crema closed.");
        }

        private static ServiceInfo[] GetServiceInfo(string address)
        {
            var serviceClient = DescriptorServiceFactory.CreateServiceClient(address);
            serviceClient.Open();
            try
            {
                var version = serviceClient.GetVersion();
                return serviceClient.GetServiceInfos();
            }
            finally
            {
                serviceClient.Close();
            }
        }

        private string ConfigPath
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var productName = AppUtility.ProductName;
                var address = this.address.Replace(':', '-');
                return Path.Combine(path, productName, $"{this.userID}@{address}.config");
            }
        }

        private LogService Log
        {
            get
            {
                if (this.log == null)
                    throw new InvalidOperationException(Resources.Exception_NotOpened);
                return this.log;
            }
        }

        internal Guid AuthenticationToken
        {
            get { return this.userContext.AuthenticationToken; }
        }

        #region ILogService

        LogVerbose ILogService.Verbose
        {
            get { return this.Log.Verbose; }
            set
            {
                this.Log.Verbose = value;
                this.verbose = value;
            }
        }

        TextWriter ILogService.RedirectionWriter
        {
            get { return this.Log.RedirectionWriter; }
            set { this.Log.RedirectionWriter = value; }
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

        string ICremaHost.Address
        {
            get { return this.address; }
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

        //IUser ICremaHost.User
        //{
        //    get { return this.User; }
        //}

        #endregion
    }
}
