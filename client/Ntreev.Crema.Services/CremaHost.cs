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
using System.Xml;

namespace Ntreev.Crema.Services
{
    [Export(typeof(ILogService))]
    [InheritedExport(typeof(ICremaHost))]
    class CremaHost : ICremaHost, IServiceProvider, ILogService
    {
        private readonly List<Authentication> authentications = new List<Authentication>();
        private readonly List<ICremaService> services = new List<ICremaService>();
        private CloseInfo closeInfo;

        private CremaConfiguration configs;
        private IEnumerable<IPlugin> plugins;

        [Import]
        private IServiceProvider container = null;
        private LogService log;
        private Guid token;
        private LogVerbose verbose;

        [ImportingConstructor]
        public CremaHost()
        {
            this.Dispatcher = new CremaDispatcher(this);
            CremaLog.Debug($"available tags : {string.Join(",", TagInfoUtility.Names)}");
            CremaLog.Debug("Crema created.");
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == typeof(ICremaHost))
                return this;
            if (serviceType == typeof(IDataBaseCollection))
                return this.DataBases;
            if (serviceType == typeof(IUserContext))
                return this.UserContext;
            if (serviceType == typeof(IUserCollection))
                return this.UserContext.Users;
            if (serviceType == typeof(IUserCategoryCollection))
                return this.UserContext.Categories;
            if (serviceType == typeof(IDomainContext))
                return this.DomainContext;
            if (serviceType == typeof(IDomainCollection))
                return this.DomainContext.Domains;
            if (serviceType == typeof(IDomainCategoryCollection))
                return this.DomainContext.Categories;
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
                await this.Dispatcher.InvokeAsync(() =>
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
            try
            {
                if (this.IsOpened == true)
                    throw new InvalidOperationException(Resources.Exception_AlreadyConnected);

                this.IPAddress = AddressUtility.GetIPAddress(address);
                this.ServiceInfos = GetServiceInfo(address).ToDictionary(item => item.Name);

                this.OnOpening(EventArgs.Empty);
                return this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        this.Address = AddressUtility.GetDisplayAddress(address);
                        this.log = new LogService(this.Address.Replace(':', '_'), userID, AppUtility.UserAppDataPath)
                        {
                            Verbose = this.verbose
                        };
                        this.UserContext = new UserContext(this, this.IPAddress, ServiceInfos[nameof(UserService)], userID, password);
                        var user = this.UserContext.Users[userID];
                        user.SetUserState(UserState.Online);
                        this.UserID = userID;
                        this.Authority = user.Authority;

                        this.DataBases = new DataBaseCollection(this, this.IPAddress, ServiceInfos[nameof(DataBaseCollectionService)]);
                        this.DomainContext = new DomainContext(this, this.IPAddress, ServiceInfos[nameof(DomainService)]);
                        this.IsOpened = true;
                        this.configs = new CremaConfiguration(this.ConfigPath);
                        this.plugins = this.container.GetService(typeof(IEnumerable<IPlugin>)) as IEnumerable<IPlugin>;
                        foreach (var item in this.plugins)
                        {
                            var authentication = new Authentication(new AuthenticationProvider(user), item.ID);
                            this.authentications.Add(authentication);
                            item.Initialize(authentication);
                        }

                        this.OnOpened(EventArgs.Empty);
                        this.token = Guid.NewGuid();
                        CremaLog.Info($"Crema opened : {address} {userID}");
                        return token;
                    }
                    catch
                    {
                        this.UserContext?.Close(CloseInfo.Empty);
                        this.UserContext = null;
                        this.log?.Dispose();
                        this.log = null;
                        this.Address = null;
                        throw;
                    }
                });
            }
            catch (Exception e)
            {
                CremaLog.Error(e);
                throw;
            }
        }

        public void SaveConfigs()
        {
            this.configs.Commit();
        }

        public void Close(Guid token)
        {
            if (this.token != token)
                throw new ArgumentException(Resources.Exception_InvalidToken, nameof(token));
            if (this.IsOpened == false)
                throw new InvalidOperationException(Resources.Exception_NotConnected);

            this.Dispatcher.Invoke(() =>
            {
                this.Close(CloseInfo.Empty);
                this.token = Guid.Empty;
            });
        }

        public void Shutdown(Authentication authentication, int milliseconds, ShutdownType shutdownType, string message)
        {
            this.DebugMethod(authentication, this, nameof(Shutdown), this, milliseconds, shutdownType, message);
            var result = this.UserContext.Service.Shutdown(milliseconds, shutdownType, message);
            result.Validate();
        }

        public void CancelShutdown(Authentication authentication)
        {
            this.DebugMethod(authentication, this, nameof(CancelShutdown));
            var result = this.UserContext.Service.CancelShutdown();
            result.Validate();
        }

        public void Dispose()
        {
            this.ValidateDispose();

            if (Environment.ExitCode != 0 && this.IsOpened == true)
            {
                this.Close(CloseInfo.Empty);
            }

            this.Dispatcher.Dispose(false);
            this.Dispatcher = null;
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

        // mac의 mono 환경에서는 바인딩 값이 서버와 다를 경우 접속이 거부되는 현상이 있음(버그로 추정)
        // binding.SentTimeout 값이 달라도 접속이 안됨.
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
            if (isDebug == true && Environment.OSVersion.Platform != PlatformID.Unix)
            {
                binding.SendTimeout = TimeSpan.MaxValue;
            }
#endif

            return binding;
        }

        public ConfigurationBase Configs => this.configs;

        public string Address { get; private set; }

        public string UserID { get; private set; }

        public Authority Authority { get; private set; }

        public User User
        {
            get
            {
                if (this.IsOpened == false)
                    return null;
                return this.UserContext.Users[this.UserID];
            }
        }

        public bool IsOpened { get; private set; }

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

        public DataBaseCollection DataBases { get; private set; }

        public DomainContext DomainContext { get; private set; }

        public UserContext UserContext { get; private set; }

        public CremaDispatcher Dispatcher { get; private set; }

        public Dictionary<string, ServiceInfo> ServiceInfos { get; private set; }

        public string IPAddress { get; private set; }

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
            if (this.Dispatcher == null)
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
            this.DomainContext = null;
            this.DataBases = null;
            this.UserContext = null;
            foreach (var item in this.plugins)
            {
                item.Release();
            }
            foreach (var item in this.authentications)
            {
                item.InvokeExpiredEvent(Authentication.SystemID);
            }
            this.Dispatcher.InvokeAsync(() =>
            {
                this.log?.Dispose();
                this.log = null;
            });
            this.Address = null;
            this.UserID = null;
            this.IsOpened = false;
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
                var address = this.Address.Replace(':', '-');
                return Path.Combine(path, productName, $"{this.UserID}@{address}.config");
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

        internal Guid AuthenticationToken => this.UserContext.AuthenticationToken;

        #region ILogService

        LogVerbose ILogService.Verbose
        {
            get => this.Log.Verbose;
            set
            {
                this.Log.Verbose = value;
                this.verbose = value;
            }
        }

        TextWriter ILogService.RedirectionWriter
        {
            get => this.Log.RedirectionWriter;
            set => this.Log.RedirectionWriter = value;
        }

        string ILogService.Name => this.log.Name;

        string ILogService.FileName => this.log.FileName;

        bool ILogService.IsEnabled => this.IsOpened;

        #endregion

        #region ICremaHost

        string ICremaHost.Address => this.Address;

        IDataBaseCollection ICremaHost.DataBases
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.DataBases;
            }
        }

        ICremaConfiguration ICremaHost.Configs => this.configs;

        #endregion
    }
}
